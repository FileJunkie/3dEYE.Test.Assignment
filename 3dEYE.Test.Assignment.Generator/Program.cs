using _3dEYE.Test.Assignment.Generator;
using CommandLine;

await Parser.Default.ParseArguments<Arguments>(args)
    .WithParsedAsync(RandomFileGenerator.GenerateAsync);
