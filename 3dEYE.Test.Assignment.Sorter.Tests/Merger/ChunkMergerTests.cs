using _3dEYE.Test.Assignment.Common.Models;
using _3dEYE.Test.Assignment.Sorter.Merger;
using FluentAssertions;
using NSubstitute;

namespace _3dEYE.Test.Assignment.Sorter.Tests.Merger;

public class ChunkMergerTests
{
    [Fact]
    public async Task Merge_Works()
    {
        // Arrange
        var randomStrings = Enumerable.Range(0, 100).Select(_ => NextRandomLine()).ToList();
        var parsedLines = randomStrings.Select(ParsedLine.Parse).ToList();
        var sortedStrings = parsedLines
            .Order()
            .Select(l => (string)l)
            .ToList();
        var chunks = parsedLines.Chunk(7);
        var chunkEnumerables = chunks.Select(c => c.ToAsyncEnumerable());
        var textWriter = Substitute.For<TextWriter>();
        var output = new List<string>();
        textWriter
            .When(x => x.WriteLineAsync(Arg.Any<string>()))
            .Do(x => output.Add(x.ArgAt<string>(0)));

        // Act
        await ChunkMerger.MergeAsync(chunkEnumerables, textWriter);

        // Assert
        output.Should().BeEquivalentTo(sortedStrings);
    }

    private static string NextRandomLine()
    {
        return $"{Random.Shared.Next()}. {NextRandomString()}";
    }

    private static string NextRandomString()
    {
        var str = Guid.NewGuid().ToString();
        return str[..Random.Shared.Next(2, str.Length)];
    }
}