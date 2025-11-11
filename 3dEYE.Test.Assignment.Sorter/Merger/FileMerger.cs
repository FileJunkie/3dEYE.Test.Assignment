using _3dEYE.Test.Assignment.Sorter.Parser;

namespace _3dEYE.Test.Assignment.Sorter.Merger;

public class FileMerger(Arguments arguments)
{
    public async Task MergeChunksAsync(List<string> chunkFiles)
    {
        await using var outputFile = File.OpenWrite(arguments.OutputFilePath);
        await using var writer = new StreamWriter(outputFile);

        await ChunkMerger.MergeAsync(chunkFiles.Select(FileParser.ParseAsync), writer);
        Console.WriteLine($"Merged into {arguments.OutputFilePath}");
        foreach (var chunkFile in chunkFiles)
        {
            File.Delete(chunkFile);
        }
    }
}