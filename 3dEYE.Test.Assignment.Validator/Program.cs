using _3dEYE.Test.Assignment.Validator;
using CommandLine;

await Parser.Default.ParseArguments<Arguments>(args)
    .WithParsedAsync(SortedFileValidator.ValidateFileAsync);