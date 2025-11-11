using _3dEYE.Test.Assignment.Common.Models;

namespace _3dEYE.Test.Assignment.Sorter.Parser;

public static class FileParser
{
    public static async IAsyncEnumerable<ParsedLine> ParseAsync(string inputFileName)
    {
        await using var inputFile = File.OpenRead(inputFileName);
        using var reader = new StreamReader(inputFile);
        while (true)
        {
            var line = await reader.ReadLineAsync();
            if (line == null)
            {
                yield break;
            }

            yield return ParsedLine.Parse(line);
        }
    }

    public static async IAsyncEnumerable<ParsedLine> ParseChunkAsync(StreamReader reader, long chunkSize)
    {
        // this will lead to slightly more bytes being read as requested since the string won't align,
        // it's fine for us unless file contains a string as large as a chunk
        var bytesRead = 0;
        while (bytesRead < chunkSize)
        {
            var line = await reader.ReadLineAsync();
            if (line == null)
            {
                yield break;
            }
            bytesRead += line.Length;

            yield return ParsedLine.Parse(line);
        }
    }
}