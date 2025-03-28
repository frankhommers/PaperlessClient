// Copyright 2022 Valters Melnalksnis
// Licensed under the Apache License 2.0.
// See LICENSE file in the project root for full license information.
// ---
// Copyright 2025 Frank Hommers

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Paperless.Correspondents;
using Paperless.Tags;

namespace Paperless.Tests.Integration.Tags;

public sealed class TagClientTests(PaperlessFixture paperlessFixture) : PaperlessTests(paperlessFixture)
{
  [Test]
  public async Task CreateGetDeleteAsync()
  {
    Tag createdTag = await Client.Tags.CreateAsync(
      new("Foo bar")
      {
        Match = "foo",
        MatchingAlgorithm = MatchingAlgorithm.ExactMatch,
        IsInsensitive = true,
        IsInboxTag = true,
      });

    Tag tag = (await Client.Tags.GetAsync(createdTag.Id))!;
    List<Tag> tags = await Client.Tags.GetAllAsync().ToListAsync();
    List<Tag> paginatedTags = await Client.Tags.GetAllAsync(1).ToListAsync();

    using (new AssertionScope())
    {
      tag.Should().BeEquivalentTo(createdTag, ExcludingDocumentCount);
      tags.Should().ContainSingle(t => t.Id == tag.Id).Which.Should().BeEquivalentTo(tag, ExcludingDocumentCount);
      paginatedTags.Should().BeEquivalentTo(tags);

      createdTag.Name.Should().Be("Foo bar");
      createdTag.Slug.Should().Be("foo-bar");
      createdTag.Match.Should().Be("foo");
      createdTag.MatchingAlgorithm.Should().Be(MatchingAlgorithm.ExactMatch);
      createdTag.IsInsensitive.Should().BeTrue();
      createdTag.IsInboxTag.Should().BeTrue();

      createdTag.DocumentCount.Should().Be(null);
      tag.DocumentCount.Should().Be(0);
    }

    await Client.Tags.DeleteAsync(createdTag.Id);
    tags = await Client.Tags.GetAllAsync().ToListAsync();

    tags.Should().NotContainEquivalentOf(createdTag);
  }

  private static EquivalencyAssertionOptions<Tag> ExcludingDocumentCount(EquivalencyAssertionOptions<Tag> options) =>
    options.Excluding(tag => tag.DocumentCount);
}
