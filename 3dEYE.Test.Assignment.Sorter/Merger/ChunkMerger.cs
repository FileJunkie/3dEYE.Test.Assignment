using _3dEYE.Test.Assignment.Common.Models;

namespace _3dEYE.Test.Assignment.Sorter.Merger;

public static class ChunkMerger
{
    public static async Task MergeAsync(
        IEnumerable<IAsyncEnumerable<ParsedLine>> lineSources,
        TextWriter outputFileWriter)
    {
        var enumerators = await CreateAndInitEnumerators(lineSources);

        while (enumerators.Count > 0)
        {
            var enumerator = enumerators.Dequeue();
            await outputFileWriter.WriteLineAsync(enumerator.Current);

            if (await enumerator.MoveNextAsync())
            {
                enumerators.Enqueue(enumerator, enumerator.Current);
            }
            else
            {
                await enumerator.DisposeAsync();
            }
        }
    }

    private static async Task<PriorityQueue<IAsyncEnumerator<ParsedLine>, ParsedLine>> CreateAndInitEnumerators(IEnumerable<IAsyncEnumerable<ParsedLine>> lineSources)
    {
        var enumerators = new PriorityQueue<IAsyncEnumerator<ParsedLine>, ParsedLine>();
        foreach (var lineSource in lineSources)
        {
            var enumerator = lineSource.GetAsyncEnumerator();
            if (await enumerator.MoveNextAsync())
            {
                enumerators.Enqueue(enumerator, enumerator.Current);
            }
            else
            {
                await enumerator.DisposeAsync();
            }
        }

        return enumerators;
    }
}