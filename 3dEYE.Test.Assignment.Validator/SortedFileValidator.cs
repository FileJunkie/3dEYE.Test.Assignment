using _3dEYE.Test.Assignment.Common.Models;

namespace _3dEYE.Test.Assignment.Validator;

public static class SortedFileValidator
{
    public static async Task ValidateFileAsync(Arguments arguments)
    {
        await using var file = File.OpenRead(arguments.FilePath);
        using var reader = new StreamReader(file);
        var line1 = await reader.ReadLineAsync();
        if (line1 == null)
        {
            Console.WriteLine("File is empty?");
        }

        while (true)
        {
            var line2 = await reader.ReadLineAsync();
            if (line2 == null)
            {
                break;
            }

            var parsedLine1 = ParsedLine.Parse(line1!);
            var parsedLine2 = ParsedLine.Parse(line2);
            if (parsedLine1.CompareTo(parsedLine2) > 0)
            {
                await Console.Error.WriteLineAsync($"Wrong line order! \"{line1}\" vs \"{line2}\"");
                return;
            }

            line1 = line2;
        }

        Console.WriteLine("File looks fine");
    }
}