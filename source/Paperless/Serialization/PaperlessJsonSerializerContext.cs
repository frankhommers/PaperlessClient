// Copyright 2022 Valters Melnalksnis
// Licensed under the Apache License 2.0.
// See LICENSE file in the project root for full license information.
// ---
// Copyright 2025 Frank Hommers

using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

using Paperless.Correspondents;
using Paperless.Documents;
using Paperless.Tags;
using Paperless.Tasks;

namespace Paperless.Serialization;

/// <inheritdoc cref="JsonSerializerContext" />
[JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Default)]
[JsonSerializable(typeof(PaginatedList<Correspondent>))]
[JsonSerializable(typeof(Correspondent))]
[JsonSerializable(typeof(CorrespondentCreation))]
[JsonSerializable(typeof(PaginatedList<Document>))]
[JsonSerializable(typeof(Document))]
[JsonSerializable(typeof(List<PaperlessTask>))]
[JsonSerializable(typeof(CustomFieldCreation))]
[JsonSerializable(typeof(List<CustomField>))]
[JsonSerializable(typeof(DocumentUpdate))]
[JsonSerializable(typeof(Dictionary<string, JsonNode>))]
[JsonSerializable(typeof(List<CustomFieldValue>))]
[JsonSerializable(typeof(PaginatedList<CustomField>))]
[JsonSerializable(typeof(PaginatedList<Tag>))]
[JsonSerializable(typeof(TagCreation))]
[JsonSerializable(typeof(DocumentMetadata))]
internal sealed partial class PaperlessJsonSerializerContext : JsonSerializerContext;
