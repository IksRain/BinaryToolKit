using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Iks.BinaryToolkit;
using Xunit.Abstractions;

namespace BinaryKit_Test;

public class StreamExTest(ITestOutputHelper output)
{
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

    #region Multiple-Test

    [Fact]
    public unsafe void MultipleRead()
    {
        byte[] ints = [10, 20, 30, 40];
        Span<int> span = stackalloc int[1];
        var ptr = (int*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(ints));
        var expected = *ptr;
        using var stream = new MemoryStream(ints);
        stream.ReadManyAs(span);
        Assert.Equal(expected, span[0]);
        stream.Position = 0;
        Span<short> span2 = stackalloc short[2];
        stream.ReadManyAs(span2);
        if (span2[0] != *(short*)ptr) Assert.Fail();
        if (span2[1] != ((short*)ptr)[1]) Assert.Fail();
    }

    [Fact]
    public void MultipleWrite()
    {
        byte[] ints = [10, 20, 30, 40];
        using var stream = new MemoryStream();
        stream.WriteManyFrom(ints);
        if (stream.GetBuffer() is not [10, 20, 30, 40, ..]) Assert.Fail();
    }

    #endregion
}