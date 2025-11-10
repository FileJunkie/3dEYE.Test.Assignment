using System.Collections.Concurrent;

namespace _3dEYE.Test.Assignment.Sorter;

public sealed class Monitor : IDisposable
{
    private readonly ConcurrentDictionary<(long, long), bool> _chunkSorters = new();
    private bool _run = true;

    public Monitor()
    {
        _ = DumpInfoAsync();
    }

    public void AddSorter((long, long) id)
    {
        _chunkSorters[id] = true;
    }

    public void RemoveSorter((long, long) id)
    {
        _chunkSorters.Remove(id, out _);
    }

    public void Dispose()
    {
        _run = false;
    }
    
    private async Task DumpInfoAsync()
    {
        while (_run)
        {
            var sorterNames = _chunkSorters.Select(x => $"{x.Key.Item1}-{x.Key.Item2}").ToList();
            Console.WriteLine("{0} sorters running: {1}", sorterNames.Count, string.Join(", ", sorterNames));

            await Task.Delay(TimeSpan.FromSeconds(15));
        }

        Console.WriteLine("Okay, shutting down");
    }
}