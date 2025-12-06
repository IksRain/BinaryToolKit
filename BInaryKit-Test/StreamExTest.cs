using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Iks.BinaryToolkit;
using Xunit.Abstractions;

namespace BinaryKit_Test;

public class StreamExTest(ITestOutputHelper output)
{
    #region ReadAs Tests

    [Fact]
    public void Read_int()
    {
        const int value = 100;
        using var stream = new MemoryStream();
        Span<byte> span = stackalloc byte[4];
        MemoryMarshal.Write(span, value);
        stream.Write(span);
        stream.Position = 0;
        var readValue = stream.ReadAs<int>();
        output.WriteLine("left: " + value);
        output.WriteLine("right: " + readValue);
        Assert.Equal(value, readValue);
    }

    [Fact]
    public unsafe void Read_NormalStruct()
    {
        UnmanagedStruct value = new()
        {
            A = 1,
            B = 2,
            C = 3
        };

        using var stream = new MemoryStream();
        stream.Write(new ReadOnlySpan<byte>(&value, sizeof(UnmanagedStruct)));
        stream.Position = 0;
        var readValue = stream.ReadAs<UnmanagedStruct>();
        output.WriteLine("left: " + value);
        output.WriteLine("right: " + readValue);
        Assert.Equal(value, readValue);
    }

    [Fact]
    public unsafe void Read_UnionStruct()
    {
        const int intV = 1000;
        UnionStruct value = new()
        {
            A = intV
        };

        using var stream = new MemoryStream();
        stream.Write(new ReadOnlySpan<byte>(&value, sizeof(UnmanagedStruct)));
        stream.Position = 0;
        var readValue = stream.ReadAs<UnionStruct>();
        output.WriteLine("left: " + value);
        output.WriteLine("right: " + readValue);
        Assert.Equal(value, readValue);
        Assert.Equal(intV, readValue.A);
        Assert.Equal(*(float*)&readValue.A, readValue.B);
    }

    #endregion

    #region WriteFrom Tests

    [Fact]
    public unsafe void Write()
    {
        UnionStruct expected = new();
        UnionStruct value = default;
        using var stream = new MemoryStream();
        stream.WriteFrom(expected);
        stream.Position = 0;
        stream.GetBuffer().AsSpan(0, sizeof(UnionStruct)).CopyTo(new Span<byte>(&expected, sizeof(UnionStruct)));
        output.WriteLine("left: " + expected);
        output.WriteLine("right: " + value);
        Assert.Equal(expected, value);
    }
    #endregion
   
}