# Test assignment for 3dEYE

## Assignment

To avoid repeating the original document, tl;dr: write a tool that would sort a 100+ GB text file, with a custom string comparison algorithm.

## Usage

`dotnet run --project 3dEYE.Test.Assignment.Generator/` runs the file generator. Available CLI arguments are:

```
  --filesizeingb      (Default: 4) File size in gigabytes
  --filepath          (Default: testFile.txt) Path to the output file
  --duplicateratio    (Default: 0.01) String duplicates ratio
```

```dotnet run --project 3dEYE.Test.Assignment.Sorter/``` runs the sorter itself. Available CLI arguments are:

```
  --inputfilepath            (Default: testFile.txt) Path to the input file
  --outputfilepath           (Default: outputFile.txt) Path to the output file
  --chunksizeinmegabytes     (Default: 1024) Chunk size in megabytes
```

```dotnet run --project 3dEYE.Test.Assignment.Validator -- --filepath file-path.txt``` runs the end result validator. 

All apps also understand the standard `--help` argument.

## Assumptions

Since the goal was to show the ability to process large amounts of data, the following assumptions were made:

1. `string.CompareOrdinal` is considered OK to compare strings. Real life is more complex because of existence of languages other than English.
2. Input file is assumed sane.

## Thought process

In late 2025, sorting the whole file by loading it into memory is possible the highest-end machines, but solving the problem with raw power was obviously not the point of the task.

Typical approach for this problem is to apply the external sorting algorithm, which is the version of merge-sort one, with each read chunk stored to disk before merging. It is implemented as following:

1. File read in chunks of certain size - for simplicity's sake, we allow chunks be a little larger than requested since the last string will almost always straddle the theoretical chunk border;
2. As chunk is being loaded, every line is pre-split into integer part and the index of the beginning of the string part to speed up further comparisons during sort process;
3. Since standard .NET library does not provide parallel sorting algorithms, which would be extremely useful in modern era, `HPCsharp` is used to sort this array on all available processor cores;
4. Chunk is saved to a temporary file;
5. When all chunk files are ready, they are merged into the result file.

To take advantage of sorting and parsing processes being inherently independent, the process is pipelined - while previous chunk is being sorted and dumped, another one is already being read. Only three chunks are in memory at the same time since per-chunk sorting is already parallelized.

"String" part of the input line is not stored a separate `string` object, but as an index in the full string, to save memory.  

## Other paths taken (and some that could be taken)

1. There was an attempt to read small chunks in parallel, sort them independently in parallel and then write as a merged chunk. It wasn't faster.
2. There was an attempt to merge the chunks as they are being created on disk, with the idea that while chunks 3+ are being prepared, we can already merge first and second. It only led to I/O overhead due to chunks being read and written multiple times, and it wasn't faster.
3. The idea was considered to read the whole chunk in memory and operate list of pointers to the resulting array, which would theoretically cut memory consumption due to overhead being lower. However, first, such code would more error-prone, second, even in theory largest array that can exist in .NET would only have 2G elements, and on developer's machine even such attempts triggered OOM error. Hence this wasn't actually tried, especially since 2G of bytes is still much less information that 2G of `ParsedLine`s.
4. The idea was considered to open the whole file as a memory-mapped one and operate pointers to it, but wasn't implemented due to risks of huge and uncontrollable I/O overhead.

## Benchmarking

Test machine has AMD Ryzen 7 7840HS with 16 cores, 32GB of RAM, and was also used as a normal desktop laptop, so not all memory was available for the sorter. Test file was 128 Gb large.

Execution times for different chunk sizes were:

* 1024 Mb: 97 minutes
* 1536 Mb: 80 minutes
* 2048 Mb: crashed.