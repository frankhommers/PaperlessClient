// Copyright 2022 Valters Melnalksnis
// Licensed under the Apache License 2.0.
// See LICENSE file in the project root for full license information.
// ---
// Copyright 2025 Frank Hommers

using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;

using NodaTime;

using Paperless.Correspondents;
using Paperless.Documents;
using Paperless.Serialization;
using Paperless.Tags;
using Paperless.Tasks;

namespace Paperless.Tests.Integration;

public sealed class MinimalExampleTests : PaperlessTests
{
  private readonly IPaperlessClient _paperlessClient;

  public MinimalExampleTests(PaperlessFixture paperlessFixture)
    : base(paperlessFixture)
  {
    var options = paperlessFixture.Options;

    var httpClient = new HttpClient { BaseAddress = options.BaseAddress };
    httpClient.DefaultRequestHeaders.Add("Accept", $"{MediaTypeNames.Application.Json}; version=2");
    httpClient.DefaultRequestHeaders.Authorization = new("Token", options.Token);

    var serializerOptions = new PaperlessJsonSerializerOptions(DateTimeZoneProviders.Tzdb);
    var taskClient = new TaskClient(httpClient, serializerOptions);
    var correspondentClient = new CorrespondentClient(httpClient, serializerOptions);
    var documentClient = new DocumentClient(httpClient, serializerOptions, taskClient, options.TaskPollingDelay);
    var tagClient = new TagClient(httpClient, serializerOptions);

    _paperlessClient = new PaperlessClient(correspondentClient, documentClient, tagClient);
  }

  [Test]
  public async Task ShouldNotThrow() => await FluentActions
    .Awaiting(() => _paperlessClient.Correspondents.GetAll().ToListAsync().AsTask())
    .Should()
    .NotThrowAsync();
}
