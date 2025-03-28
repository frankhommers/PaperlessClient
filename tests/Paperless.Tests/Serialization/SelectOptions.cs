﻿// Copyright 2022 Valters Melnalksnis
// Licensed under the Apache License 2.0.
// See LICENSE file in the project root for full license information.
// ---
// Copyright 2025 Frank Hommers

using Ardalis.SmartEnum;

namespace Paperless.Tests.Serialization;

public sealed class SelectOptions : SmartEnum<SelectOptions>
{
  public static readonly SelectOptions Option1 = new("First option", 0);
  public static readonly SelectOptions Option2 = new("Second option", 1);

  private SelectOptions(string name, int value)
    : base(name, value)
  {
  }
}
