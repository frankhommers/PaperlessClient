// Copyright 2022 Valters Melnalksnis
// Licensed under the Apache License 2.0.
// See LICENSE file in the project root for full license information.
// ---
// Copyright 2025 Frank Hommers

using Paperless.Correspondents;
using Paperless.Documents;
using Paperless.Tags;

namespace Paperless;

/// <summary>All available Paperless APIs.</summary>
public interface IPaperlessClient
{
  /// <summary>Gets the documents API client.</summary>
  ICorrespondentClient Correspondents { get; }

  /// <summary>Gets the documents API client.</summary>
  IDocumentClient Documents { get; }

  /// <summary>Gets the tags API client.</summary>
  ITagClient Tags { get; }
}
