// Copyright 2022 Valters Melnalksnis
// Licensed under the Apache License 2.0.
// See LICENSE file in the project root for full license information.
// ---
// Copyright 2025 Frank Hommers

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;

using NodaTime;

using Paperless.Correspondents;
using Paperless.Documents;
using Paperless.Tags;

namespace Paperless.Tests.Integration.Documents;

public sealed class DocumentClientTests(PaperlessFixture paperlessFixture) : PaperlessTests(paperlessFixture)
{
  [Test]
  [Order(1)]
  public async Task GetAll_ShouldReturnExpected()
  {
    List<Document> documents = await Client.Documents.GetAll().ToListAsync();

    documents.Should().BeEmpty();
  }

  [Test]
  public async Task GetAll_PageSizeShouldNotChangeResult()
  {
    List<Document> documents = await Client.Documents.GetAll().ToListAsync();
    List<Document> pageSizeDocuments = await Client.Documents.GetAll(1).ToListAsync();

    documents.Should().BeEquivalentTo(pageSizeDocuments);
  }

  [Test]
  public async Task Create()
  {
    const string documentName = "Lorem Ipsum.txt";

    Correspondent correspondent = await Client.Correspondents.Create(new("Foo"));
    List<Tag> tags = new();
    foreach (TagCreation tag in new List<TagCreation> { new("Receipt"), new("Bill") })
    {
      tags.Add(await Client.Tags.Create(tag));
    }

    await using Stream documentStream = typeof(DocumentClientTests).GetResource(documentName);
    DocumentCreation documentCreation = new(documentStream, documentName)
    {
      Created = Clock.GetCurrentInstant(),
      Title = "Lorem Ipsum",
      CorrespondentId = correspondent.Id,
      ArchiveSerialNumber = 1,
      TagIds = tags.Select(tag => tag.Id).ToArray(),
    };

    DocumentCreationResult result = await Client.Documents.Create(documentCreation);

    if (PaperlessVersion < new Version(2, 0))
    {
      result.Should().BeOfType<ImportStarted>();
      return;
    }

    int id = result.Should().BeOfType<DocumentCreated>().Subject.Id;
    Document document = (await Client.Documents.Get(id))!;
    List<Document> documents = await Client.Documents.GetAll().ToListAsync();
    DocumentMetadata metadata = await Client.Documents.GetMetadata(id);

    using AssertionScope scope = new();

    Instant currentTime = SystemClock.Instance.GetCurrentInstant();
    string content = await typeof(DocumentClientTests).ReadResource(documentName);

    documents.Should().ContainSingle(d => d.Id == id).Which.Should().BeEquivalentTo(document);
    document.Should().NotBeNull();
    document.OriginalFileName.Should().Be(documentName);
    document.Created.ToInstant().Should().Be(documentCreation.Created.Value);
    document.Added.ToInstant().Should().BeInRange(currentTime - Duration.FromSeconds(10), currentTime);
    document.Modified.ToInstant().Should().BeInRange(currentTime - Duration.FromSeconds(10), currentTime);
    document.Title.Should().Be(documentCreation.Title);
    document.ArchiveSerialNumber.Should().Be(documentCreation.ArchiveSerialNumber);
    document.CorrespondentId.Should().Be(documentCreation.CorrespondentId);
    document.TagIds.Should().BeEquivalentTo(tags.Select(tag => tag.Id));
#if NET6_0_OR_GREATER
    document.Content.ReplaceLineEndings().Should().BeEquivalentTo(content);
#else
    document.Content.Replace("\n", Environment.NewLine).Replace("\r\n", Environment.NewLine).Should().Be(content);
#endif
    metadata.Should().BeEquivalentTo(
      new DocumentMetadata(
        $"{id:0000000}.txt",
        Environment.NewLine is "\r\n" ? "999853181bf31bb3f54be7c0bc20f6af" : "be37b4f97ce9f67970d878978e5db5eb",
        Environment.NewLine is "\r\n" ? 2868 : 2859,
        MediaTypeNames.Text.Plain,
        documentName,
        [],
        "ca",
        false));

    DocumentUpdate update = new()
      { Title = $"{document.Title}1" };
    Document updatedDocument = await Client.Documents.Update(id, update);

    updatedDocument.Title.Should().Be(update.Title);

    await Client.Correspondents.Delete(correspondent.Id);
    foreach (Tag tag in tags)
    {
      await Client.Tags.Delete(tag.Id);
    }

    await Client.Documents.Delete(id);

    await FluentActions
      .Awaiting(() => Client.Documents.Get(id))
      .Should()
      .ThrowExactlyAsync<HttpRequestException>()
      .WithMessage("Response status code does not indicate success: 404 (Not Found).");
  }

  [Test]
  public async Task CreateDuplicate()
  {
    if (PaperlessVersion < new Version(2, 0))
    {
      Assert.Ignore($"Paperless v{PaperlessVersion} does not directly allow downloading documents.");
    }

    const string documentName = "Lorem Ipsum 4.txt";

    IEnumerable<Task<DocumentCreationResult>> tasks = Enumerable
      .Range(1, 2)
      .Select(
        _ =>
        {
          Stream stream = typeof(DocumentClientTests).GetResource(documentName);
          DocumentCreation creation = new(stream, documentName)
          {
            Created = Clock.GetCurrentInstant(),
            Title = "Lorem Ipsum",
          };

          return Client.Documents.Create(creation);
        });

    DocumentCreationResult[] results = await Task.WhenAll(tasks);

    using AssertionScope scope = new();
    DocumentCreated? created = results.OfType<DocumentCreated>().Should().ContainSingle().Subject;
    results
      .OfType<ImportFailed>()
      .Should()
      .HaveCount(results.Length - 1)
      .And.AllSatisfy(
        failed =>
          failed.Result.Should()
            .Be(
              $"{documentName}: Not consuming {documentName}: It is a duplicate of Lorem Ipsum (#{created.Id})."));

    await Client.Documents.Delete(created.Id);
  }

  [Test]
  public async Task Download()
  {
    if (PaperlessVersion < new Version(2, 0))
    {
      Assert.Ignore($"Paperless v{PaperlessVersion} does not directly allow downloading documents.");
    }

    const string documentName = "Lorem Ipsum 3.txt";

    await using Stream documentStream = typeof(DocumentClientTests).GetResource(documentName);
    DocumentCreation documentCreation = new(documentStream, documentName);

    DocumentCreationResult createResult = await Client.Documents.Create(documentCreation);
    int id = createResult.Should().BeOfType<DocumentCreated>().Subject.Id;

    string expectedDocumentContent = await typeof(DocumentClientTests).ReadResource(documentName);
    string expectedPartOfFileName = "Lorem Ipsum 3";

    // Download
    DocumentContent documentDownload = await Client.Documents.Download(id);
    documentDownload.MediaTypeHeaderValue.MediaType.Should().Be("text/plain");
    documentDownload.ContentDisposition!.FileName.Should().Contain(expectedPartOfFileName);

    string downloadContent = await ReadStreamContentAsString(documentDownload.Content);
    downloadContent.Should().BeEquivalentTo(expectedDocumentContent);

    // Download Original
    DocumentContent documentOriginalDownload = await Client.Documents.DownloadOriginal(id);
    documentOriginalDownload.MediaTypeHeaderValue.MediaType.Should().Be("text/plain");
    documentOriginalDownload.ContentDisposition!.FileName.Should().Contain(expectedPartOfFileName);

    string downloadOriginalContent = await ReadStreamContentAsString(documentOriginalDownload.Content);
    downloadOriginalContent.Should().BeEquivalentTo(expectedDocumentContent);

    // Download Preview
    DocumentContent downloadPreview = await Client.Documents.DownloadPreview(id);
    downloadPreview.MediaTypeHeaderValue.MediaType.Should().Be("text/plain");
    downloadPreview.ContentDisposition!.FileName.Should().Contain(expectedPartOfFileName);

    string downloadPreviewContent = await ReadStreamContentAsString(downloadPreview.Content);
    downloadPreviewContent.Should().BeEquivalentTo(expectedDocumentContent);

    // Download Preview Original
    DocumentContent downloadPreviewOriginal = await Client.Documents.DownloadPreview(id);
    downloadPreviewOriginal.MediaTypeHeaderValue.MediaType.Should().Be("text/plain");
    downloadPreviewOriginal.ContentDisposition!.FileName.Should().Contain(expectedPartOfFileName);

    string downloadPreviewOrignalContent = await ReadStreamContentAsString(downloadPreviewOriginal.Content);
    downloadPreviewOrignalContent.Should().BeEquivalentTo(expectedDocumentContent);

    // Download thumbnail
    DocumentContent downloadThumbnail = await Client.Documents.DownloadThumbnail(id);
    downloadThumbnail.MediaTypeHeaderValue.MediaType.Should().Be("image/webp");
  }

  [Test]
  public async Task CustomFields()
  {
    if (PaperlessVersion < new Version(2, 0))
    {
      Assert.Ignore($"Paperless v{PaperlessVersion} does not support custom fields");
    }

    const string documentName = "Lorem Ipsum 2.txt";

    List<CustomFieldCreation> fieldCreations = new()
    {
      new("field1", CustomFieldType.String),
      new("field2", CustomFieldType.Url),
      new("field3", CustomFieldType.Date),
      new("field4", CustomFieldType.Boolean),
      new("field5", CustomFieldType.Integer),
      new("field6", CustomFieldType.Float),
      new("field7", CustomFieldType.Monetary),
      new("field8", CustomFieldType.DocumentLink),
    };

    if (PaperlessVersion >= new Version(2, 11, 0))
    {
      fieldCreations.Add(new SelectCustomFieldCreation<SelectOptions>("field9"));
    }

    foreach (CustomFieldCreation customFieldCreation in fieldCreations)
    {
      await Client.Documents.CreateCustomField(customFieldCreation);
    }

    List<CustomField> customFields = await Client.Documents.GetCustomFields().ToListAsync();
    customFields.Should().HaveCount(fieldCreations.Count);

    List<CustomField> paginatedCustomFields = await Client.Documents.GetCustomFields(1).ToListAsync();
    paginatedCustomFields.Should().BeEquivalentTo(customFields);

    await using Stream documentStream = typeof(DocumentClientTests).GetResource(documentName);
    DocumentCreation documentCreation = new(documentStream, documentName);

    DocumentCreationResult result = await Client.Documents.Create(documentCreation);

    SerializerOptions.CustomFields.Clear();

    int id = result.Should().BeOfType<DocumentCreated>().Subject.Id;
    Document<CustomFields>? document = (await Client.Documents.Get<CustomFields>(id))!;

    document.CustomFields.Should().BeNull("cannot create document with custom fields");

    DocumentUpdate<CustomFields> update = new()
    {
      CustomFields = new()
      {
        Field1 = "foo",
        Field2 = new("https://example.com/"),
        Field3 = new LocalDate(2024, 01, 19),
        Field4 = true,
        Field5 = 12,
        Field6 = 12.34567f,
        Field7 = 12.34f,
        Field8 = [id],
      },
    };

    if (PaperlessVersion >= new Version(2, 11, 0))
    {
      update.CustomFields.Field9 = SelectOptions.Option1;
    }

    SerializerOptions.CustomFields.Clear();
    document = await Client.Documents.Update(id, update);

    document.CustomFields.Should().BeEquivalentTo(update.CustomFields);

    SerializerOptions.CustomFields.Clear();
    List<Document<CustomFields>> documents = await Client.Documents.GetAll<CustomFields>().ToListAsync();
    documents.Should().ContainSingle(d => d.Id == id).Which.Should().BeEquivalentTo(document);
  }

  private async Task<string> ReadStreamContentAsString(Stream stream)
  {
    await using (stream)
    {
      StreamReader reader = new(stream);

      return await reader.ReadToEndAsync();
    }
  }
}
