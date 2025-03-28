// Copyright 2022 Valters Melnalksnis
// Licensed under the Apache License 2.0.
// See LICENSE file in the project root for full license information.
// ---
// Copyright 2025 Frank Hommers

using System.Text.Json.Serialization;

using Paperless.Documents;

namespace Paperless.Tests.Serialization;

/// <inheritdoc cref="JsonSerializerContext" />
[JsonSerializable(typeof(Document<TestCustomFields>))]
internal sealed partial class TestSerializerContext : JsonSerializerContext;
