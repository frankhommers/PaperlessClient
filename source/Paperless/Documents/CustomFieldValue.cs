﻿// Copyright 2022 Valters Melnalksnis
// Licensed under the Apache License 2.0.
// See LICENSE file in the project root for full license information.
// ---
// Copyright 2025 Frank Hommers

using System.Diagnostics;
using System.Text.Json.Nodes;

namespace Paperless.Documents;

[DebuggerDisplay("{Field} - {Value}")]
internal sealed class CustomFieldValue
{
  public JsonNode Value { get; set; } = null!;

  public int Field { get; set; }
}
