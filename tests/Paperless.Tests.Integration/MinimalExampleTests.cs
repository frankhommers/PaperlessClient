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
    PaperlessOptions? options = paperlessFixture.Options;

    HttpClient? httpClient = new()
      { BaseAddress = options.BaseAddress };
    httpClient.DefaultRequestHeaders.Add("Accept", $"{MediaTypeNames.Application.Json}; version=2");
    httpClient.DefaultRequestHeaders.Authorization = new("Token", options.Token);

    PaperlessJsonSerializerOptions serializerOptions = new(DateTimeZoneProviders.Tzdb);
    TaskClient taskClient = new(httpClient, serializerOptions);
    CorrespondentClient correspondentClient = new(httpClient, serializerOptions);
    DocumentClient documentClient = new(httpClient, serializerOptions, taskClient, options.TaskPollingDelay);
    TagClient tagClient = new(httpClient, serializerOptions);

    _paperlessClient = new PaperlessClient(correspondentClient, documentClient, tagClient);
  }

  [Test]
  public async Task ShouldNotThrowAsync() => await FluentActions
    .Awaiting(() => _paperlessClient.Correspondents.GetAllAsync().ToListAsync().AsTask())
    .Should()
    .NotThrowAsync();
}
