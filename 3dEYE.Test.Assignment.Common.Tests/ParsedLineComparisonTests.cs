using _3dEYE.Test.Assignment.Common.Models;
using FluentAssertions;

namespace _3dEYE.Test.Assignment.Common.Tests;

public class ParsedLineComparisonTests
{
    [Theory]
    [InlineData("1. Line", "1. Line", 0)]
    [InlineData("1. Line", "2. Line", -1)]
    [InlineData("2. Line", "1. Line", 1)]
    [InlineData("1. AAA", "1. BBB", -1)]
    [InlineData("3. AAA", "2. BBB", -1)]
    public void Comparer_MustWorkCorrectly(string input1, string input2, int expectedResult)
    {
        // Arrange
        var parsed1 = ParsedLine.Parse(input1);
        var parsed2 = ParsedLine.Parse(input2);

        // Act
        var result = parsed1.CompareTo(parsed2);

        // Assert
        if (expectedResult < 0)
        {
            result.Should().BeNegative();
        }
        else if (expectedResult == 0)
        {
            result.Should().Be(0);
        }
        else
        {
            result.Should().BePositive();
        }
    }
}