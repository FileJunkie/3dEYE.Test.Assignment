namespace _3dEYE.Test.Assignment.Sorter;

public class ChunkMerger(string inputFileName1, string inputFileName2)
{
    public string OutputFileName { get; } = $"chunk_{Guid.NewGuid()}.txt";

    public async Task MergeAsync()
    {
        await using var inputFile1 = File.OpenRead(inputFileName1);
        using var inputFile1Reader = new StreamReader(inputFile1);
        await using var inputFile2 = File.OpenRead(inputFileName2);
        using var inputFile2Reader = new StreamReader(inputFile2);

        await using var outputFile = File.OpenWrite(OutputFileName);
        await using var writer = new StreamWriter(outputFile);
        var input1 = await inputFile1Reader.ReadLineAsync();
        var input2 = await inputFile2Reader.ReadLineAsync();
        while (input1 != null || input2 != null)
        {
            if (input1 == null)
            {
                while (input2 != null)
                {
                    await writer.WriteLineAsync(input2);
                    input2 = await inputFile2Reader.ReadLineAsync();
                }

                break;
            }

            if (input2 == null)
            {
                while (input1 != null)
                {
                    await writer.WriteLineAsync(input1);
                    input1 = await inputFile2Reader.ReadLineAsync();
                }

                break;
            }

            if (StringComparer.Instance.Compare(input1, input2) < 0)
            {
                await writer.WriteLineAsync(input1);
                input1 = await inputFile1Reader.ReadLineAsync();
            }
            else
            {
                await writer.WriteLineAsync(input2);
                input2 = await inputFile2Reader.ReadLineAsync();
            }
        }
    }
}