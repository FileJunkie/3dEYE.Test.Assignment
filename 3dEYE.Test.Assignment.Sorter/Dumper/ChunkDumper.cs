using _3dEYE.Test.Assignment.Sorter.Models;

namespace _3dEYE.Test.Assignment.Sorter.Dumper;

public static class ChunkDumper
{
    public static async Task DumpChunk(ChunkFile chunkFile)
    {
        Console.WriteLine($"Dumping {chunkFile.FilePath}");
        await using var outputFile = File.OpenWrite(chunkFile.FilePath);
        await using var writer = new StreamWriter(outputFile);
        foreach (var line in chunkFile.Lines)
        {
            await writer.WriteLineAsync(line);
        }
        Console.WriteLine($"Dumped {chunkFile.FilePath}");
    }
}