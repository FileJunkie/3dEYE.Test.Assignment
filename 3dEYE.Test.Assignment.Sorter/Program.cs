using System.Threading.Tasks.Dataflow;
using _3dEYE.Test.Assignment.Sorter;
using Monitor = _3dEYE.Test.Assignment.Sorter.Monitor;

const string filePath = "testFile.txt";
await using var file = File.OpenRead(filePath);
const long chunkSize = 32L * 1024L * 1024L;

var monitor = new Monitor();
var fileNames = new List<string>();

var chunkSorterBlock = new TransformBlock<(string, long, long), string>(
    async t1 =>
    {
        var (fileName, start, end) = t1;
        monitor.AddSorter((start, end));
        var chunkSorter = new ChunkSorter(fileName, start, end);
        await chunkSorter.SortAsync();
        monitor.RemoveSorter((start, end));
        return chunkSorter.OutputFileName;
    }, new ExecutionDataflowBlockOptions
    {
        MaxDegreeOfParallelism = 16,
    });

var chunkFilenameSaverBlock = new ActionBlock<string>(fileName => fileNames.Add(fileName),
    new()
    {
        MaxDegreeOfParallelism = 1,
    });

chunkSorterBlock.LinkTo(chunkFilenameSaverBlock);

var lastChunkBorder = 0L;
while (file.Position < file.Length)
{
    file.Position = Math.Min(lastChunkBorder + chunkSize, file.Length);
    if (file.Position != file.Length)
    {
        while (file.ReadByte() != '\n')
        {
            file.Position -= 2;
        }
    }

    Console.WriteLine("Chunk is {0}-{1}",  lastChunkBorder, file.Position);
    await chunkSorterBlock.SendAsync((filePath, lastChunkBorder, file.Position));
    lastChunkBorder = file.Position;
}

chunkSorterBlock.Complete();
while (true)
{
    if (chunkSorterBlock.InputCount == 0)
    {
        break;
    }
    Console.WriteLine("Input count: {0}", chunkSorterBlock.InputCount);
    await Task.Delay(TimeSpan.FromSeconds(15));
}
await chunkSorterBlock.Completion;

Console.WriteLine($"Got {fileNames.Count} files");

while (fileNames.Count > 1)
{
    var newFileNames = new List<string>();

    var mergerBlock = new TransformBlock<(string, string), string>(async t =>
    {
        var (file1, file2) = t;
        var merger = new ChunkMerger(file1, file2);
        await merger.MergeAsync();
        File.Delete(file1);
        File.Delete(file2);
        Console.WriteLine($"{file1} and {file2} deleted");
        return merger.OutputFileName;
    }, new()
    {
        MaxDegreeOfParallelism = 16,
    });

    var mergedFilenameSaverBlock = new ActionBlock<string>(fileName => newFileNames.Add(fileName),
        new()
        {
            MaxDegreeOfParallelism = 1,
        });

    mergerBlock.LinkTo(mergedFilenameSaverBlock);

    Console.WriteLine($"Got {fileNames.Count} files");
    Console.WriteLine($"Files are {string.Join(", ", fileNames)}");
    var i = 0;
    for (; i < fileNames.Count - 1; i += 2)
    {
        Console.WriteLine($"Merging {fileNames[i]} with {fileNames[i + 1]}");
        await mergerBlock.SendAsync((fileNames[i], fileNames[i + 1]));
    }

    string[] leftoverFiles = [];
    if (i == fileNames.Count - 1)
    {
        leftoverFiles = [fileNames[i]];
        Console.WriteLine($"Leftover file is {leftoverFiles.Single()}");
    }

    mergerBlock.Complete();
    await mergerBlock.Completion;
    fileNames = newFileNames.Concat(leftoverFiles).ToList();
}

Console.WriteLine($"Surviving file is {fileNames.Single()}");