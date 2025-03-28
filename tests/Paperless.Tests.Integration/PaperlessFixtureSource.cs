// Copyright 2022 Valters Melnalksnis
// Licensed under the Apache License 2.0.
// See LICENSE file in the project root for full license information.
// ---
// Copyright 2025 Frank Hommers

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Paperless.Tests.Integration;

public sealed class PaperlessFixtureSource : IEnumerable<TestFixtureData>
{
  /// <inheritdoc />
  public IEnumerator<TestFixtureData> GetEnumerator() => PaperlessSetup
    .Fixtures
    .Select(fixture => new TestFixtureData(fixture).SetArgDisplayNames(fixture.Name))
    .GetEnumerator();

  /// <inheritdoc />
  IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
