namespace _3dEYE.Test.Assignment.Sorter;

public class ChunkSorter(string inputFileName, long start, long end)
{
    public string OutputFileName => $"chunk_{start}-{end}.txt";
    public async Task SortAsync()
    {
        await using var inputFile = File.OpenRead(inputFileName);
        await using var limitedFile = new LimitedStream(inputFile, start, end);
        using var reader = new StreamReader(limitedFile);
        var lines = new List<string>();
        while (true)
        {
            var line = await reader.ReadLineAsync();
            if (string.IsNullOrWhiteSpace(line))
            {
                break;
            }

            lines.Add(line);
        }

        Console.WriteLine("Chunk read, position {0}/{1}", inputFile.Position, inputFile.Length);
        lines.Sort(StringComparer.Instance);
        Console.WriteLine("Lines sorted");

        await using var outputFile = File.OpenWrite(OutputFileName);
        await using var writer = new StreamWriter(outputFile);
        foreach (var line in lines)
        {
            await writer.WriteLineAsync(line);
        }
        Console.WriteLine($"Chunk written into {OutputFileName}");
    }
}