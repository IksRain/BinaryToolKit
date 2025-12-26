using System.Buffers.Binary;
using System.Runtime.InteropServices;
using Iks.BinaryToolkit;
using Xunit.Abstractions;

namespace BinaryKit_Test;

public class EndianTest(ITestOutputHelper output)
{
    [Fact]
    public void Read_int()
    {
        const int value = 1000;
        var expected = BitConverter.IsLittleEndian ? value : BinaryPrimitives.ReverseEndianness(value);
        var actual = value;
        actual = EndianToolkit.Convert(actual, Endianness.Local, Endianness.Little);
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
        if (BitConverter.IsLittleEndian) new Span<byte>(&expected, sizeof(UnionStruct)).Reverse();
        // to big endian
        actual = EndianToolkit.Convert(actual, Endianness.Local, Endianness.Big);
        output.WriteLine("expected: " + expected);
        output.WriteLine("actual: " + actual);
        Assert.Equal(expected, actual);
    }

    #region Multiple-Test

    [Fact]
    public void ReverseManyTest()
    {
        Span<int> old = [10, 20, 30, 40, 50, 60, 70, 80];
        Span<int> expected = stackalloc int[old.Length];
        for (var index = 0; index < old.Length; index++)
        {
            var item = old[index];
            expected[index] = BinaryPrimitives.ReverseEndianness(item);
        }

        EndianToolkit.ReverseMany(old);
        for (var i = 0; i < old.Length; i++)
            if (old[i] != expected[i])
                Assert.Fail();
    }
    [Fact]
    public unsafe void ReverseManyTestButWithPointer()
    {
        Span<int> old = [10, 20, 30, 40, 50, 60, 70, 80];
        Span<int> expected = stackalloc int[old.Length];
        for (var index = 0; index < old.Length; index++)
        {
            var item = old[index];
            expected[index] = BinaryPrimitives.ReverseEndianness(item);
        }
        fixed(void* ptr = &MemoryMarshal.GetReference(old))
            EndianToolkit.ReverseMany(ptr,sizeof(int),old.Length);
        for (var i = 0; i < old.Length; i++)
            if (old[i] != expected[i])
                Assert.Fail();
    }
    [Fact]
    public void ReverseManyButSize5WithPointer()
    {
        // 5 elements * 4(sizeof(int)) = 20 bytes
        Span<int> old = [10, 20, 30, 40, 50];
        // 4 elements * 5(sizeof(Size5Struct)) = 20 bytes
        Span<Size5Struct> expected = stackalloc Size5Struct[4];
        // set expected
        old.CopyTo(MemoryMarshal.Cast<Size5Struct,int>(expected));
        MemoryMarshal.Cast<Size5Struct,byte>(expected).Reverse();
        expected.Reverse();
        // init actual
        Span<int> actual = stackalloc int[old.Length];
        old.CopyTo(actual);
        // to reverse
        unsafe
        {
            fixed (int* ptr = &MemoryMarshal.GetReference(actual))
            {
                EndianToolkit.ReverseMany(ptr,5,4);
            }
        }
        // check
        if (!MemoryMarshal.Cast<Size5Struct,byte>(expected).SequenceEqual(MemoryMarshal.Cast<int,byte>(actual)))
        {
            Assert.Fail();
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    struct Size5Struct
    {
        [FieldOffset(0)]
        public unsafe fixed byte Inner[5];
    }

    #endregion
}