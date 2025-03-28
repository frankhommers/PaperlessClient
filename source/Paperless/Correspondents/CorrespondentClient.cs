// Copyright 2022 Valters Melnalksnis
// Licensed under the Apache License 2.0.
// See LICENSE file in the project root for full license information.
// ---
// Copyright 2025 Frank Hommers

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Paperless.Serialization;

namespace Paperless.Correspondents;

/// <inheritdoc />
public sealed class CorrespondentClient : ICorrespondentClient
{
  private readonly HttpClient _httpClient;
  private readonly JsonSerializerOptions _options;

  /// <summary>Initializes a new instance of the <see cref="CorrespondentClient"/> class.</summary>
  /// <param name="httpClient">Http client configured for making requests to the Paperless API.</param>
  /// <param name="serializerOptions">Paperless specific instance of <see cref="JsonSerializerOptions"/>.</param>
  public CorrespondentClient(HttpClient httpClient, PaperlessJsonSerializerOptions serializerOptions)
  {
    _httpClient = httpClient;
    _options = serializerOptions.Options;
  }

  /// <inheritdoc />
  public IAsyncEnumerable<Correspondent> GetAll(CancellationToken cancellationToken = default) =>
    GetAllCore(Routes.Correspondents.Uri, cancellationToken);

  /// <inheritdoc />
  public IAsyncEnumerable<Correspondent> GetAll(int pageSize, CancellationToken cancellationToken = default) =>
    GetAllCore(Routes.Correspondents.PagedUri(pageSize), cancellationToken);

  /// <inheritdoc />
  public Task<Correspondent?> Get(int id, CancellationToken cancellationToken = default) =>
    _httpClient.GetFromJsonAsync(
      Routes.Correspondents.IdUri(id),
      _options.GetTypeInfo<Correspondent>(),
      cancellationToken);

  /// <inheritdoc />
  public Task<Correspondent> Create(CorrespondentCreation correspondent) => _httpClient.PostAsJsonAsync(
    Routes.Correspondents.Uri,
    correspondent,
    _options.GetTypeInfo<CorrespondentCreation>(),
    _options.GetTypeInfo<Correspondent>());

  /// <inheritdoc />
  public async Task Delete(int id)
  {
    using HttpResponseMessage? response =
      await _httpClient.DeleteAsync(Routes.Correspondents.IdUri(id)).ConfigureAwait(false);
    await response.EnsureSuccessStatusCodeAsync().ConfigureAwait(false);
  }

  private IAsyncEnumerable<Correspondent> GetAllCore(Uri requestUri, CancellationToken cancellationToken) =>
    _httpClient.GetPaginated(
      requestUri,
      _options.GetTypeInfo<PaginatedList<Correspondent>>(),
      cancellationToken);
}
