using Iks.BinaryToolkit;
using Xunit.Abstractions;

namespace BinaryKit_Test;

public class EndianTest(ITestOutputHelper output)
{
    [Fact]
    public void Read_int()
    {
        const int value = 1000;
        var expected = BitConverter.IsLittleEndian ? value : System.Buffers.Binary.BinaryPrimitives.ReverseEndianness(value);
        var actual = value;
        EndianToolkit.Convert(ref actual, Endianness.Local, Endianness.Little);
        output.WriteLine("expected: " + expected);
        output.WriteLine("actual: " + actual);
        Assert.Equal(expected, actual);
    }
    
    [Fact]
    public unsafe void Read_Struct()
    {
        UnionStruct value = new();
        var expected = value;
        var actual = value;
        // to big endian
        if (BitConverter.IsLittleEndian)
        {
            new Span<byte>(&expected, sizeof(UnionStruct)).Reverse();
        }
        // to big endian
        EndianToolkit.Convert(ref actual, Endianness.Local, Endianness.Big);
        output.WriteLine("expected: " + expected);
        output.WriteLine("actual: " + actual);
        Assert.Equal(expected, actual);
    }
    
}