namespace _3dEYE.Test.Assignment.Generator;

public static class RandomFileGenerator
{
    public static async Task GenerateAsync(Arguments arguments)
    {
        await using var file = new FileStream(arguments.FileName, FileMode.Create);
        await using var textWriter = new StreamWriter(file);

        var targetFileSize = arguments.FileSizeInGb * 1024L * 1024L * 1024L;
        Console.WriteLine($"Target file size is {targetFileSize} bytes");

        var lastString = NextRandomString();
        var loggingStep = targetFileSize / 1000;
        var nextLoggingStep = 0L;
        while (file.Position < targetFileSize)
        {
            if (file.Position > nextLoggingStep)
            {
                nextLoggingStep += loggingStep;
                Console.WriteLine($"{file.Position}/{targetFileSize} bytes");
            }

            var newString = $"{Random.Shared.Next()}. {lastString}";
            await textWriter.WriteLineAsync(newString);

            if (Random.Shared.NextDouble() > arguments.DuplicateRatio)
            {
                lastString = NextRandomString();
            }
        }

        Console.WriteLine("Done");
    }

    private static string NextRandomString()
    {
        var str = Guid.NewGuid().ToString();
        return str[..Random.Shared.Next(2, str.Length)];
    }
}