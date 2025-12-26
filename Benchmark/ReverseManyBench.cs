using System.Buffers.Binary;
using BenchmarkDotNet.Attributes;
using Iks.BinaryToolkit;

namespace Benchmark;

[MemoryDiagnoser]
[RankColumn]
[MarkdownExporter]
public class ReverseManyBench
{
    [Params(1000000L,100000000L)]
    public long Size = 1000000;
    
    [GlobalSetup]
    public void SetUpGlobal()
    {
        _stdOutput = new int[Size];
        _mine = new int[Size];
        _std = new int[Size];
    }
    
    private int[]? _mine;
    private int[]? _std;
    private int[]? _stdOutput;
    [Benchmark]
    public void StandardReverse()
    {
        for (int i = 0; i < 10; i++)
        {
            BinaryPrimitives.ReverseEndianness(_std, _stdOutput);
        }
       
    }
    [Benchmark]
    public void MineReverse()
    {
        for (int i = 0; i < 10; i++)
        {
            EndianToolkit.ReverseMany(_mine);
        }
    }
}