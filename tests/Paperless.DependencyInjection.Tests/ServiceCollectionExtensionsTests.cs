// Copyright 2022 Valters Melnalksnis
// Licensed under the Apache License 2.0.
// See LICENSE file in the project root for full license information.
// ---
// Copyright 2025 Frank Hommers

using System;
using System.Collections.Generic;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using NodaTime;

namespace Paperless.DependencyInjection.Tests;

public sealed class ServiceCollectionExtensionsTests
{
  private readonly IConfiguration _configuration = new ConfigurationBuilder()
    .AddInMemoryCollection(new List<KeyValuePair<string, string?>>
    {
      new($"{PaperlessOptions.Name}:{nameof(PaperlessOptions.BaseAddress)}", "https://localhost:5002/"),
      new($"{PaperlessOptions.Name}:{nameof(PaperlessOptions.Token)}", Guid.NewGuid().ToString("N"))
    })
    .Build();

  [Fact]
  public void AddPaperlessClient_ExplicitConfiguration_ShouldRegisterRequiredServices()
  {
    var serviceCollection = new ServiceCollection();

    serviceCollection
      .AddSingleton<IClock>(SystemClock.Instance)
      .AddSingleton(DateTimeZoneProviders.Tzdb)
      .AddPaperlessClient(_configuration);

    using var serviceProvider = serviceCollection.BuildServiceProvider();
    var paperlessClient = serviceProvider.GetRequiredService<IPaperlessClient>();

    paperlessClient.Should().NotBeNull();
  }

  [Fact]
  public void AddPaperlessClient_ShouldRegisterRequiredServices()
  {
    var serviceCollection = new ServiceCollection();

    serviceCollection
      .AddSingleton(_configuration)
      .AddSingleton<IClock>(SystemClock.Instance)
      .AddSingleton(DateTimeZoneProviders.Tzdb)
      .AddPaperlessClient();

    using var serviceProvider = serviceCollection.BuildServiceProvider();
    var paperlessClient = serviceProvider.GetRequiredService<IPaperlessClient>();

    paperlessClient.Should().NotBeNull();
  }
}
