using _3dEYE.Test.Assignment.Sorter;
using _3dEYE.Test.Assignment.Sorter.Sorter;
using CommandLine;

await Parser.Default.ParseArguments<Arguments>(args)
    .WithParsedAsync(async arguments =>
    {
        var fileSorter = new FileSorter(arguments);
        await fileSorter.SortFileAsync();
    });
