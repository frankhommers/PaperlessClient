﻿// Copyright 2022 Valters Melnalksnis
// Licensed under the Apache License 2.0.
// See LICENSE file in the project root for full license information.
// ---
// Copyright 2025 Frank Hommers

namespace Paperless.Correspondents;

/// <summary>Information needed to create a new <see cref="Correspondent"/>.</summary>
public sealed class CorrespondentCreation
{
  /// <summary>Initializes a new instance of the <see cref="CorrespondentCreation"/> class.</summary>
  /// <param name="name">The name of the correspondent.</param>
  public CorrespondentCreation(string name)
  {
    Name = name;
  }

  /// <inheritdoc cref="Correspondent.Slug"/>
  public string? Slug { get; set; }

  /// <inheritdoc cref="Correspondent.Name"/>
  public string Name { get; set; }

  /// <inheritdoc cref="Correspondent.MatchingPattern"/>
  public string? Match { get; set; }

  /// <inheritdoc cref="Correspondent.MatchingAlgorithm"/>
  public MatchingAlgorithm? MatchingAlgorithm { get; set; }

  /// <inheritdoc cref="Correspondent.IsInsensitive"/>
  public bool? IsInsensitive { get; set; }
}
