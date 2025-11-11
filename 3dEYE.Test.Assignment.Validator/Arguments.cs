using CommandLine;

namespace _3dEYE.Test.Assignment.Validator;

public class Arguments
{
    [Option(HelpText = "Path to the input file", Required = true)]
    public required string FilePath { get; init; }
}