namespace _3dEYE.Test.Assignment.Common.Models;

public readonly record struct ParsedLine(string OriginalString, int IntPart, int StringPartStart) : IComparable<ParsedLine>
{
    public static ParsedLine Parse(string str)
    {
        // Working with string as span minimizes amount of pointless object allocations and data duplication
        var strAsSpan = str.AsSpan();

        using var splitEnumerator = strAsSpan.Split('.').GetEnumerator();

        splitEnumerator.MoveNext();
        var intPart = int.Parse(strAsSpan[splitEnumerator.Current].Trim());

        splitEnumerator.MoveNext();
        var stringPartStart = splitEnumerator.Current.Start.Value;
        while (stringPartStart < str.Length && char.IsWhiteSpace(strAsSpan[stringPartStart]))
        {
            stringPartStart++;
        }

        return new(str, intPart, stringPartStart);
    }

    public int CompareTo(ParsedLine other)
    {
        var ordinalCompare = string.CompareOrdinal(
            OriginalString,
            StringPartStart,
            other.OriginalString,
            other.StringPartStart,
            int.MaxValue);
        if (ordinalCompare != 0)
        {
            return ordinalCompare;
        }

        return IntPart - other.IntPart;
    }

    public static implicit operator string(ParsedLine parsedLine) => parsedLine.OriginalString;
}