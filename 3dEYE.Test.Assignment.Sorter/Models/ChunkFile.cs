using _3dEYE.Test.Assignment.Common.Models;

namespace _3dEYE.Test.Assignment.Sorter.Models;

public readonly record struct ChunkFile(string FilePath, ParsedLine[] Lines);