using CommandLine;

namespace _3dEYE.Test.Assignment.Generator;

public class Arguments
{
    // TODO validation
    [Option(HelpText = "File size in gigabytes", Default = 4)]
    public required int FileSizeInGb { get; init; }

    [Option(HelpText = "Path to the output file", Default = "testFile.txt")]
    public required string FilePath { get; init; }

    // TODO validation
    [Option(HelpText = "String duplicates ratio", Default = 0.01)]
    public required double DuplicateRatio { get; init; }
}