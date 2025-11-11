using CommandLine;

namespace _3dEYE.Test.Assignment.Sorter;

public class Arguments
{
    [Option(HelpText = "Path to the input file", Default = "testFile.txt")]
    public required string InputFilePath { get; init; }

    [Option(HelpText = "Path to the output file", Default = "outputFile.txt")]
    public required string OutputFilePath { get; init; }

    // TODO validation
    [Option(HelpText = "Chunk size in megabytes", Default = 1024)]
    public required int ChunkSizeInMegabytes { get; init; }
}