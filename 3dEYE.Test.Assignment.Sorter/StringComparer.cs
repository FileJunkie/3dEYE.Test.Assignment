namespace _3dEYE.Test.Assignment.Sorter;

public class StringComparer : IComparer<string>
{
    public static StringComparer Instance { get; } = new();

    public int Compare(string? x, string? y)
    {
        if (string.IsNullOrWhiteSpace(x) || string.IsNullOrWhiteSpace(y))
        {
            throw new Exception();
        }

        var splitX = x.Split('.').Select(s => s.Trim()).ToArray();
        var splitY = y.Split('.').Select(s => s.Trim()).ToArray();
        if (splitX[1] != splitY[1])
        {
            return string.CompareOrdinal(splitX[1], splitY[1]);
        }

        var intX = int.Parse(splitX[0]);
        var intY = int.Parse(splitY[0]);
        return intX - intY;
    }
}