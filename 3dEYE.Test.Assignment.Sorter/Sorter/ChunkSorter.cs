using _3dEYE.Test.Assignment.Sorter.Models;

namespace _3dEYE.Test.Assignment.Sorter.Sorter;

public static class ChunkSorter
{
    public static ChunkFile Sort(ChunkFile chunkFile)
    {
        Console.WriteLine($"Sorting {chunkFile.FilePath}");
        HPCsharp.ParallelAlgorithm.SortMergeInPlaceAdaptivePar(chunkFile.Lines);
        Console.WriteLine($"Sorted {chunkFile.FilePath}");
        return chunkFile;
    }
}