using System.Threading.Tasks.Dataflow;
using _3dEYE.Test.Assignment.Sorter.Dumper;
using _3dEYE.Test.Assignment.Sorter.Merger;
using _3dEYE.Test.Assignment.Sorter.Models;
using _3dEYE.Test.Assignment.Sorter.Parser;

namespace _3dEYE.Test.Assignment.Sorter.Sorter;

public class FileSorter(Arguments arguments)
{
    private readonly long _chunkSize = arguments.ChunkSizeInMegabytes * 1024L * 1024L;
    private readonly FileMerger _fileMerger = new(arguments);

    public async Task SortFileAsync()
    {
        await using var inputFile = File.OpenRead(arguments.InputFilePath);
        using var fileReader = new StreamReader(inputFile);

        var linesSorter = CreateLinesSorter();
        var linesWriter = PrepareLinesWriter();
        linesSorter.LinkTo(linesWriter);

        var chunkFiles = await ProcessInputFileInChunksAsync(fileReader, linesSorter, inputFile).ToListAsync();
        Console.WriteLine("File fully read");

        linesSorter.Complete();
        await linesSorter.Completion;
        Console.WriteLine("Sorting done");

        linesWriter.Complete();
        await linesWriter.Completion;
        Console.WriteLine("Writing done");

        Console.WriteLine($"{chunkFiles.Count} chunks prepared");

        await _fileMerger.MergeChunksAsync(chunkFiles);
        Console.WriteLine($"Chunks merged, done");
    }

    private async IAsyncEnumerable<string> ProcessInputFileInChunksAsync(
        StreamReader fileReader,
        TransformBlock<ChunkFile, ChunkFile> linesSorter,
        FileStream inputFile)
    {
        while (true)
        {
            var outputFileName = $"chunk_{Guid.NewGuid()}.txt";
            Console.WriteLine($"Starting {outputFileName}...");
            var lines = await FileParser
                .ParseChunkAsync(fileReader, _chunkSize)
                .ToArrayAsync();
            if (lines.Length == 0)
            {
                Console.WriteLine($"Not creating file {outputFileName}, it would be empty");
                yield break;
            }

            Console.WriteLine($"Posting {outputFileName}...");
            linesSorter.Post(new(outputFileName, lines));
            Console.WriteLine($"Posted. File position is {inputFile.Position}/{inputFile.Length} ({inputFile.Position * 100.0 / inputFile.Length:F1}%)");
            yield return outputFileName;
        }
    }

    private static ActionBlock<ChunkFile> PrepareLinesWriter() =>
        new(
            ChunkDumper.DumpChunk,
            new()
            {
                BoundedCapacity = 1,
                MaxDegreeOfParallelism = 1,
            });

    private static TransformBlock<ChunkFile, ChunkFile> CreateLinesSorter() =>
        new (
            ChunkSorter.Sort,
            new()
            {
                BoundedCapacity = 1,
                MaxDegreeOfParallelism = 1,
            });
}