﻿// Copyright 2022 Valters Melnalksnis
// Licensed under the Apache License 2.0.
// See LICENSE file in the project root for full license information.
// ---
// Copyright 2025 Frank Hommers

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Paperless.Correspondents;

/// <summary>Paperless API client for working with correspondents.</summary>
public interface ICorrespondentClient
{
  /// <summary>Gets all correspondents.</summary>
  /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
  /// <returns>A enumerable which will asynchronously iterate over all available pages of correspondents.</returns>
  IAsyncEnumerable<Correspondent> GetAllAsync(CancellationToken cancellationToken = default);

  /// <summary>Gets all correspondents.</summary>
  /// <param name="pageSize">The number of correspondents to get in a single request.</param>
  /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
  /// <returns>A enumerable which will asynchronously iterate over all available pages of correspondents.</returns>
  IAsyncEnumerable<Correspondent> GetAllAsync(int pageSize, CancellationToken cancellationToken = default);

  /// <summary>Gets the correspondent with the specified id.</summary>
  /// <param name="id">The id of the correspondent to get.</param>
  /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
  /// <returns>The correspondent with the specified id if it exists; otherwise <see langword="null"/>.</returns>
  Task<Correspondent?> GetAsync(int id, CancellationToken cancellationToken = default);

  /// <summary>Creates a new correspondent.</summary>
  /// <param name="correspondent">The correspondent to create.</param>
  /// <returns>The created correspondent.</returns>
  Task<Correspondent> CreateAsync(CorrespondentCreation correspondent);

  /// <summary>Deletes a correspondent.</summary>
  /// <param name="id">The id of the correspondent to delete.</param>
  /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
  Task DeleteAsync(int id);
}
