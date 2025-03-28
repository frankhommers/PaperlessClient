// Copyright 2022 Valters Melnalksnis
// Licensed under the Apache License 2.0.
// See LICENSE file in the project root for full license information.
// ---
// Copyright 2025 Frank Hommers

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using NodaTime.Text;

using Paperless.Serialization;
using Paperless.Tasks;

namespace Paperless.Documents;

/// <inheritdoc />
public sealed class DocumentClient : IDocumentClient
{
  private static readonly Version _documentIdVersion = new(1, 9, 2);

  private readonly HttpClient _httpClient;
  private readonly ITaskClient _taskClient;
  private readonly TimeSpan _taskPollingDelay;
  private readonly JsonSerializerOptions _options;
  private readonly PaperlessJsonSerializerOptions _paperlessOptions;

  /// <summary>Initializes a new instance of the <see cref="DocumentClient"/> class.</summary>
  /// <param name="httpClient">Http client configured for making requests to the Paperless API.</param>
  /// <param name="serializerOptions">Paperless specific instance of <see cref="JsonSerializerOptions"/>.</param>
  /// <param name="taskClient">Paperless task API client.</param>
  /// <param name="taskPollingDelay">The delay in ms between polling for import task completion.</param>
  public DocumentClient(
    HttpClient httpClient,
    PaperlessJsonSerializerOptions serializerOptions,
    ITaskClient taskClient,
    TimeSpan taskPollingDelay)
  {
    _httpClient = httpClient;
    _taskClient = taskClient;
    _taskPollingDelay = taskPollingDelay;
    _options = serializerOptions.Options;
    _paperlessOptions = serializerOptions;
  }

  /// <inheritdoc />
  public IAsyncEnumerable<Document> GetAllAsync(CancellationToken cancellationToken = default) =>
    GetAllCoreAsync<Document>(Routes.Documents.Uri, cancellationToken);

  /// <inheritdoc />
  public IAsyncEnumerable<Document> GetAllAsync(DocumentFilter filter, CancellationToken cancellationToken = default)
  {
    Uri uri = new(Routes.Documents.Uri.ToString() + filter.ToQueryString());
    return GetAllCoreAsync<Document>(uri, cancellationToken);
  }

  /// <inheritdoc />
  public async IAsyncEnumerable<Document<TFields>> GetAllAsync<TFields>(
    [EnumeratorCancellation] CancellationToken cancellationToken = default)
  {
    if (_paperlessOptions.CustomFields.Count is 0)
    {
      await foreach (CustomField? unused in GetCustomFieldsAsync(cancellationToken).ConfigureAwait(false))
      {
      }
    }

    IAsyncEnumerable<Document<TFields>>? documents = GetAllCoreAsync<Document<TFields>>(
      Routes.Documents.Uri,
      cancellationToken);
    await foreach (Document<TFields>? document in documents.ConfigureAwait(false))
    {
      yield return document;
    }
  }

  /// <inheritdoc />
  public async IAsyncEnumerable<Document<TFields>> GetAllAsync<TFields>(
    DocumentFilter filter,
    [EnumeratorCancellation] CancellationToken cancellationToken = default)
  {
    if (_paperlessOptions.CustomFields.Count is 0)
    {
      await foreach (CustomField? unused in GetCustomFieldsAsync(cancellationToken).ConfigureAwait(false))
      {
      }
    }

    Uri uri = new(Routes.Documents.Uri.ToString() + filter.ToQueryString());
    IAsyncEnumerable<Document<TFields>> documents = GetAllCoreAsync<Document<TFields>>(uri, cancellationToken);
    await foreach (Document<TFields>? document in documents.ConfigureAwait(false))
    {
      yield return document;
    }
  }

  /// <inheritdoc />
  public IAsyncEnumerable<Document> GetAllAsync(int pageSize, CancellationToken cancellationToken = default) =>
    GetAllCoreAsync<Document>(Routes.Documents.PagedUri(pageSize), cancellationToken);

  /// <inheritdoc />
  public IAsyncEnumerable<Document> GetAllAsync(
    DocumentFilter filter,
    int pageSize,
    CancellationToken cancellationToken = default)
  {
    Uri uri = new(Routes.Documents.PagedUri(pageSize).ToString() + filter.ToQueryString());
    return GetAllCoreAsync<Document>(uri, cancellationToken);
  }

  /// <inheritdoc />
  public async IAsyncEnumerable<Document<TFields>> GetAllAsync<TFields>(
    int pageSize,
    [EnumeratorCancellation] CancellationToken cancellationToken = default)
  {
    if (_paperlessOptions.CustomFields.Count is 0)
    {
      await foreach (CustomField? unused in GetCustomFieldsAsync(cancellationToken).ConfigureAwait(false))
      {
      }
    }

    IAsyncEnumerable<Document<TFields>>? documents = GetAllCoreAsync<Document<TFields>>(
      Routes.Documents.PagedUri(pageSize),
      cancellationToken);
    await foreach (Document<TFields>? document in documents.ConfigureAwait(false))
    {
      yield return document;
    }
  }

  /// <inheritdoc />
  public async IAsyncEnumerable<Document<TFields>> GetAllAsync<TFields>(
    DocumentFilter filter,
    int pageSize,
    [EnumeratorCancellation] CancellationToken cancellationToken = default)
  {
    if (_paperlessOptions.CustomFields.Count is 0)
    {
      await foreach (CustomField? unused in GetCustomFieldsAsync(cancellationToken).ConfigureAwait(false))
      {
      }
    }

    Uri uri = new(Routes.Documents.PagedUri(pageSize).ToString() + filter.ToQueryString());
    IAsyncEnumerable<Document<TFields>> documents = GetAllCoreAsync<Document<TFields>>(uri, cancellationToken);
    await foreach (Document<TFields>? document in documents.ConfigureAwait(false))
    {
      yield return document;
    }
  }

  /// <inheritdoc />
  public Task<Document?> GetAsync(int id, CancellationToken cancellationToken = default) =>
    GetCoreAsync<Document>(id, cancellationToken);

  /// <inheritdoc />
  public async Task<Document<TFields>?> GetAsync<TFields>(int id, CancellationToken cancellationToken = default)
  {
    if (_paperlessOptions.CustomFields.Count is 0)
    {
      await foreach (CustomField? unused in GetCustomFieldsAsync(cancellationToken).ConfigureAwait(false))
      {
      }
    }

    return await GetCoreAsync<Document<TFields>>(id, cancellationToken).ConfigureAwait(false);
  }

  /// <inheritdoc />
  public async Task<DocumentMetadata> GetMetadataAsync(int id, CancellationToken cancellationToken = default)
  {
    DocumentMetadata? metadata = await _httpClient
      .GetFromJsonAsync(
        Routes.Documents.MetadataUri(id),
        _options.GetTypeInfo<DocumentMetadata>(),
        cancellationToken)
      .ConfigureAwait(false);

    return metadata!;
  }

  /// <inheritdoc />
  public async Task<DocumentContent> DownloadAsync(int id, CancellationToken cancellationToken = default) =>
    await DownloadContentCoreAsync(Routes.Documents.DownloadUri(id), cancellationToken).ConfigureAwait(false);

  /// <inheritdoc />
  public async Task<DocumentContent> DownloadOriginalAsync(int id, CancellationToken cancellationToken = default) =>
    await DownloadContentCoreAsync(Routes.Documents.DownloadOriginalUri(id), cancellationToken).ConfigureAwait(false);

  /// <inheritdoc />
  public async Task<DocumentContent> DownloadPreviewAsync(int id, CancellationToken cancellationToken = default) =>
    await DownloadContentCoreAsync(Routes.Documents.DownloadPreview(id), cancellationToken).ConfigureAwait(false);

  /// <inheritdoc />
  public async Task<DocumentContent> DownloadOriginalPreviewAsync(int id, CancellationToken cancellationToken = default) =>
    await DownloadContentCoreAsync(Routes.Documents.DownloadOriginalPreview(id), cancellationToken)
      .ConfigureAwait(false);

  /// <inheritdoc />
  public async Task<DocumentContent> DownloadThumbnailAsync(int id, CancellationToken cancellationToken = default) =>
    await DownloadContentCoreAsync(Routes.Documents.DownloadThumbnail(id), cancellationToken).ConfigureAwait(false);

  /// <inheritdoc />
  public async Task<DocumentCreationResult> CreateAsync(DocumentCreation document)
  {
    MultipartFormDataContent content = new();
    content.Add(new StreamContent(document.Document), "document", document.FileName);

    if (document.Title is { } title)
    {
      content.Add(new StringContent(title), "title");
    }

    if (document.Created is { } created)
    {
      content.Add(new StringContent(InstantPattern.General.Format(created)), "created");
    }

    if (document.CorrespondentId is { } correspondent)
    {
      content.Add(new StringContent(correspondent.ToString()), "correspondent");
    }

    if (document.DocumentTypeId is { } documentType)
    {
      content.Add(new StringContent(documentType.ToString()), "document_type");
    }

    if (document.StoragePathId is { } storagePath)
    {
      content.Add(new StringContent(storagePath.ToString()), "storage_path");
    }

    foreach (int tag in document.TagIds ?? [])
    {
      content.Add(new StringContent(tag.ToString()), "tags");
    }

    if (document.ArchiveSerialNumber is { } archiveSerialNumber)
    {
      content.Add(new StringContent(archiveSerialNumber.ToString()), "archive_serial_number");
    }

    using HttpResponseMessage? response =
      await _httpClient.PostAsync(Routes.Documents.CreateUri, content).ConfigureAwait(false);
    await response.EnsureSuccessStatusCodeAsync().ConfigureAwait(false);

    // Until v1.9.2 paperless did not return the document import task id,
    // so it is not possible to get the document id
    string? versionHeader = response.Headers.GetValues("x-version").SingleOrDefault();
    if (versionHeader is null ||
        !Version.TryParse(versionHeader, out Version? version) ||
        version <= _documentIdVersion)
    {
      return new ImportStarted();
    }

    Guid id = await response.Content.ReadFromJsonAsync(_options.GetTypeInfo<Guid>()).ConfigureAwait(false);
    PaperlessTask? task = await _taskClient.GetAsync(id).ConfigureAwait(false);

    while (task is not null && !task.Status.IsCompleted)
    {
      await Task.Delay(_taskPollingDelay).ConfigureAwait(false);
      task = await _taskClient.GetAsync(id).ConfigureAwait(false);
    }

    return task switch
    {
      null => new ImportFailed($"Could not find the import task by the given id {id}"),
      _ when task.RelatedDocument is { } documentId => new DocumentCreated(documentId),
      _ when task.Status == PaperlessTaskStatus.Success => new ImportFailed(
        $"Task status is {PaperlessTaskStatus.Success.Name}, but document id was not given"),
      _ when task.Status == PaperlessTaskStatus.Failure => new ImportFailed(task.Result),
      _ => throw new ArgumentOutOfRangeException(nameof(task.Status), task.Status, "Unexpected task result"),
    };
  }

  /// <inheritdoc />
  public Task<Document> UpdateAsync(int id, DocumentUpdate document) => UpdateCoreAsync<Document, DocumentUpdate>(id, document);

  /// <inheritdoc />
  public async Task<Document<TFields>> UpdateAsync<TFields>(int id, DocumentUpdate<TFields> document)
  {
    if (_paperlessOptions.CustomFields.Count is 0)
    {
      await foreach (CustomField? unused in GetCustomFieldsAsync().ConfigureAwait(false))
      {
      }
    }

    return await UpdateCoreAsync<Document<TFields>, DocumentUpdate<TFields>>(id, document).ConfigureAwait(false);
  }

  /// <inheritdoc />
  public async Task DeleteAsync(int id)
  {
    using HttpResponseMessage? response =
      await _httpClient.DeleteAsync(Routes.Documents.IdUri(id)).ConfigureAwait(false);
    await response.EnsureSuccessStatusCodeAsync().ConfigureAwait(false);
  }

  /// <inheritdoc />
  public IAsyncEnumerable<CustomField> GetCustomFieldsAsync(CancellationToken cancellationToken = default) =>
    GetCustomFieldsCoreAsync(Routes.CustomFields.Uri, cancellationToken);

  /// <inheritdoc />
  public IAsyncEnumerable<CustomField> GetCustomFieldsAsync(int pageSize, CancellationToken cancellationToken = default) =>
    GetCustomFieldsCoreAsync(Routes.CustomFields.PagedUri(pageSize), cancellationToken);

  /// <inheritdoc />
  public async Task<CustomField> CreateCustomFieldAsync(CustomFieldCreation field)
  {
    using HttpResponseMessage? response = await _httpClient
      .PostAsJsonAsync(Routes.CustomFields.Uri, field, _options.GetTypeInfo<CustomFieldCreation>())
      .ConfigureAwait(false);

    await response.EnsureSuccessStatusCodeAsync().ConfigureAwait(false);

    CustomField? createdField = (await response.Content.ReadFromJsonAsync(_options.GetTypeInfo<CustomField>())
      .ConfigureAwait(false))!;
    _paperlessOptions.CustomFields.AddOrUpdate(createdField.Id, createdField, (_, _) => createdField);

    return createdField;
  }

  private IAsyncEnumerable<TDocument> GetAllCoreAsync<TDocument>(Uri requestUri, CancellationToken cancellationToken)
    where TDocument : Document => _httpClient.GetPaginatedAsync(
    requestUri,
    _options.GetTypeInfo<PaginatedList<TDocument>>(),
    cancellationToken);

  private Task<TDocument?> GetCoreAsync<TDocument>(int id, CancellationToken cancellationToken)
    where TDocument : Document => _httpClient.GetFromJsonAsync(
    Routes.Documents.IdUri(id),
    _options.GetTypeInfo<TDocument>(),
    cancellationToken);

  private async Task<TDocument> UpdateCoreAsync<TDocument, TUpdate>(int id, TUpdate update)
    where TDocument : Document
    where TUpdate : DocumentUpdate
  {
    using HttpResponseMessage? response = await _httpClient
      .PatchAsJsonAsync(Routes.Documents.IdUri(id), update, _options.GetTypeInfo<TUpdate>())
      .ConfigureAwait(false);

    await response.EnsureSuccessStatusCodeAsync().ConfigureAwait(false);

    return (await response.Content.ReadFromJsonAsync(_options.GetTypeInfo<TDocument>()).ConfigureAwait(false))!;
  }

  private async Task<DocumentContent> DownloadContentCoreAsync(
    Uri requestUri,
    CancellationToken cancellationToken = default)
  {
    HttpResponseMessage? response = await _httpClient.GetAsync(requestUri, cancellationToken).ConfigureAwait(false);

    await response.EnsureSuccessStatusCodeAsync().ConfigureAwait(false);

    HttpContentHeaders? headers = response.Content.Headers;

    return new(
#if NETSTANDARD2_0
      await response.Content.ReadAsStreamAsync().ConfigureAwait(false),
#else
      await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false),
#endif
      headers.ContentDisposition,
      headers.ContentType!);
  }

  private async IAsyncEnumerable<CustomField> GetCustomFieldsCoreAsync(
    Uri requestUri,
    [EnumeratorCancellation] CancellationToken cancellationToken)
  {
    IAsyncEnumerable<CustomField>? fields = _httpClient.GetPaginatedAsync(
      requestUri,
      _options.GetTypeInfo<PaginatedList<CustomField>>(),
      cancellationToken);

    await foreach (CustomField? field in fields.ConfigureAwait(false))
    {
      _paperlessOptions.CustomFields.AddOrUpdate(field.Id, field, (_, _) => field);
      yield return field;
    }
  }
}
