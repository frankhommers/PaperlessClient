// Copyright 2022 Valters Melnalksnis
// Licensed under the Apache License 2.0.
// See LICENSE file in the project root for full license information.
// ---
// Copyright 2025 Frank Hommers

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Paperless.Documents;

/// <summary>Paperless API client for working with documents.</summary>
public interface IDocumentClient
{
  /// <summary>Gets all documents.</summary>
  /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
  /// <returns>An enumerable which will asynchronously iterate over all available pages of documents.</returns>
  IAsyncEnumerable<Document> GetAllAsync(CancellationToken cancellationToken = default);

  /// <summary>Gets all documents.</summary>
  /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
  /// <typeparam name="TFields">The type containing the custom fields.</typeparam>
  /// <returns>An enumerable which will asynchronously iterate over all available pages of documents.</returns>
  IAsyncEnumerable<Document<TFields>> GetAllAsync<TFields>(CancellationToken cancellationToken = default);

  /// <summary>Gets all documents.</summary>
  /// <param name="pageSize">The number of documents to get in a single request.</param>
  /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
  /// <returns>An enumerable which will asynchronously iterate over all available pages of documents.</returns>
  IAsyncEnumerable<Document> GetAllAsync(int pageSize, CancellationToken cancellationToken = default);

  /// <summary>Gets all documents.</summary>
  /// <param name="pageSize">The number of documents to get in a single request.</param>
  /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
  /// <typeparam name="TFields">The type containing the custom fields.</typeparam>
  /// <returns>An enumerable which will asynchronously iterate over all available pages of documents.</returns>
  IAsyncEnumerable<Document<TFields>> GetAllAsync<TFields>(int pageSize, CancellationToken cancellationToken = default);

  /// <summary>Gets all documents with the specified filter.</summary>
  /// <param name="filter">The filter to apply to the documents.</param>
  /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
  /// <returns>All documents matching the filter.</returns>
  IAsyncEnumerable<Document> GetAllAsync(DocumentFilter filter, CancellationToken cancellationToken = default);

  /// <summary>Gets all documents with custom fields with the specified filter.</summary>
  /// <param name="filter">The filter to apply to the documents.</param>
  /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
  /// <typeparam name="TFields">The type of custom fields.</typeparam>
  /// <returns>All documents with custom fields matching the filter.</returns>
  IAsyncEnumerable<Document<TFields>> GetAllAsync<TFields>(
    DocumentFilter filter,
    CancellationToken cancellationToken = default);

  /// <summary>Gets all documents with the specified filter and page size.</summary>
  /// <param name="filter">The filter to apply to the documents.</param>
  /// <param name="pageSize">The number of documents to get in a single request.</param>
  /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
  /// <returns>All documents matching the filter.</returns>
  IAsyncEnumerable<Document> GetAllAsync(
    DocumentFilter filter,
    int pageSize,
    CancellationToken cancellationToken = default);

  /// <summary>Gets all documents with custom fields with the specified filter and page size.</summary>
  /// <param name="filter">The filter to apply to the documents.</param>
  /// <param name="pageSize">The number of documents to get in a single request.</param>
  /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
  /// <typeparam name="TFields">The type of custom fields.</typeparam>
  /// <returns>All documents with custom fields matching the filter.</returns>
  IAsyncEnumerable<Document<TFields>> GetAllAsync<TFields>(
    DocumentFilter filter,
    int pageSize,
    CancellationToken cancellationToken = default);

  /// <summary>Gets the document with the specified id.</summary>
  /// <param name="id">The id of the document to get.</param>
  /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
  /// <returns>The document with the specified id if it exists; otherwise <see langword="null"/>.</returns>
  Task<Document?> GetAsync(int id, CancellationToken cancellationToken = default);

  /// <summary>Gets the document with the specified id.</summary>
  /// <param name="id">The id of the document to get.</param>
  /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
  /// <typeparam name="TFields">The type containing the custom fields.</typeparam>
  /// <returns>The document with the specified id if it exists; otherwise <see langword="null"/>.</returns>
  Task<Document<TFields>?> GetAsync<TFields>(int id, CancellationToken cancellationToken = default);

  /// <summary>Gets the metadata for the document with the specified id.</summary>
  /// <param name="id">The id of the document for which to get the metadata.</param>
  /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
  /// <returns>The metadata of the specified document.</returns>
  Task<DocumentMetadata> GetMetadataAsync(int id, CancellationToken cancellationToken = default);

  /// <summary>Downloads the archived file of the document.</summary>
  /// <param name="id">The id of the document to download.</param>
  /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
  /// <returns>The content of the document.</returns>
  Task<DocumentContent> DownloadAsync(int id, CancellationToken cancellationToken = default);

  /// <summary>Downloads the original file of the document.</summary>
  /// <param name="id">The id of the document to download.</param>
  /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
  /// <returns>The content of the document.</returns>
  Task<DocumentContent> DownloadOriginalAsync(int id, CancellationToken cancellationToken = default);

  /// <summary>Display the document inline, without downloading it.</summary>
  /// <param name="id">The id of the document to download.</param>
  /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
  /// <returns>The content of the document.</returns>
  Task<DocumentContent> DownloadPreviewAsync(int id, CancellationToken cancellationToken = default);

  /// <summary>Display the original document inline, without downloading it.</summary>
  /// <param name="id">The id of the document to download.</param>
  /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
  /// <returns>The content of the document.</returns>
  Task<DocumentContent> DownloadOriginalPreviewAsync(int id, CancellationToken cancellationToken = default);

  /// <summary>Download the PNG thumbnail of a document.</summary>
  /// <param name="id">The id of the document to download.</param>
  /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
  /// <returns>The content of the document.</returns>
  Task<DocumentContent> DownloadThumbnailAsync(int id, CancellationToken cancellationToken = default);

  /// <summary>Creates a new document.</summary>
  /// <param name="document">The document to create.</param>
  /// <returns>Result of creating the document.</returns>
  Task<DocumentCreationResult> CreateAsync(DocumentCreation document);

  /// <summary>Updates an existing document.</summary>
  /// <param name="id">The id of the document to update.</param>
  /// <param name="document">The fields of the document to update.</param>
  /// <returns>The updated document.</returns>
  Task<Document> UpdateAsync(int id, DocumentUpdate document);

  /// <summary>Updates an existing document.</summary>
  /// <param name="id">The id of the document to update.</param>
  /// <param name="document">The fields of the document to update.</param>
  /// <typeparam name="TFields">The type containing the custom fields.</typeparam>
  /// <returns>The updated document.</returns>
  Task<Document<TFields>> UpdateAsync<TFields>(int id, DocumentUpdate<TFields> document);

  /// <summary>Deletes the document with the specified id.</summary>
  /// <param name="id">The id of the document to delete.</param>
  /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
  Task DeleteAsync(int id);

  /// <summary>Gets all custom fields.</summary>
  /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
  /// <returns>A collection of all custom fields.</returns>
  IAsyncEnumerable<CustomField> GetCustomFieldsAsync(CancellationToken cancellationToken = default);

  /// <summary>Gets all documents.</summary>
  /// <param name="pageSize">The number of documents to get in a single request.</param>
  /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
  /// <returns>An enumerable which will asynchronously iterate over all available pages of documents.</returns>
  IAsyncEnumerable<CustomField> GetCustomFieldsAsync(int pageSize, CancellationToken cancellationToken = default);

  /// <summary>Creates a new custom field.</summary>
  /// <param name="field">The custom field to create.</param>
  /// <returns>The created field.</returns>
  Task<CustomField> CreateCustomFieldAsync(CustomFieldCreation field);
}
