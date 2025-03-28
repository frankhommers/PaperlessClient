// Copyright 2022 Valters Melnalksnis
// Licensed under the Apache License 2.0.
// See LICENSE file in the project root for full license information.
// ---
// Copyright 2025 Frank Hommers

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Paperless.Correspondents;

namespace Paperless.Tests.Integration.Correspondents;

public sealed class CorrespondentClientTests(PaperlessFixture paperlessFixture) : PaperlessTests(paperlessFixture)
{
  [Test]
  public async Task Create_ShouldCreateExpected()
  {
    CorrespondentCreation creation = new("Acme Company")
    {
      Slug = "acme-company",
      Match = "acme company",
      IsInsensitive = true,
      MatchingAlgorithm = MatchingAlgorithm.ExactMatch,
    };

    Correspondent correspondent = await Client.Correspondents.Create(creation);

    using (new AssertionScope())
    {
      correspondent.LastCorrespondence.Should().BeNull();
      correspondent.DocumentCount.Should().Be(0);
      correspondent.IsInsensitive.Should().Be(creation.IsInsensitive.Value);
      correspondent.MatchingAlgorithm.Should().Be(creation.MatchingAlgorithm);
      (await Client.Correspondents.Get(correspondent.Id))
        .Should()
        .BeEquivalentTo(correspondent);

      List<Correspondent> correspondents = await Client.Correspondents.GetAll().ToListAsync();
      correspondents.Should().ContainSingle().Which.Should().BeEquivalentTo(correspondent);
    }

    await Client.Correspondents.Delete(correspondent.Id);

    (await Client.Correspondents.GetAll().ToListAsync()).Should().BeEmpty();
  }

  [Test]
  public async Task GetAll_PageSizeShouldNotChangeResult()
  {
    List<Correspondent> correspondents = new();
    for (int i = 0; i < 5; i++)
    {
      CorrespondentCreation creation = new(Guid.NewGuid().ToString("N"));
      correspondents.Add(await Client.Correspondents.Create(creation));
    }

    using (new AssertionScope())
    {
      (await Client.Correspondents.GetAll().ToListAsync()).Should().BeEquivalentTo(correspondents);
      (await Client.Correspondents.GetAll(1).ToListAsync()).Should().BeEquivalentTo(correspondents);
    }

    foreach (Correspondent correspondent in correspondents)
    {
      await Client.Correspondents.Delete(correspondent.Id);
    }
  }
}
