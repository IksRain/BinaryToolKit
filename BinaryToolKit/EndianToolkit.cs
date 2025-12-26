using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Iks.BinaryToolkit;

/// <summary>
///     provides methods to process endianness of unmanaged types,including int,float.
/// </summary>
public static class EndianToolkit
{
    #region Mainly Code

    private static unsafe void ReverseNoCheck(void* value, int length)
    {
        switch (length)
        {
            case 1:
                return;
            case 2:
                *(ushort*)value = BinaryPrimitives.ReverseEndianness(*(ushort*)value);
                break;
            case 4:
                *(uint*)value = BinaryPrimitives.ReverseEndianness(*(uint*)value);

                break;
            case 8:
                *(ulong*)value = BinaryPrimitives.ReverseEndianness(*(ulong*)value);

                break;
            case 16:
                *(Int128*)value = BinaryPrimitives.ReverseEndianness(*(Int128*)value);
                break;
            default:
                new Span<byte>(value, length).Reverse();
                break;
        }
    }

    #endregion


    #region Single

    /// <summary>
    ///     reverses the endianness of an unmanaged type(value).
    /// </summary>
    /// <param name="value">value wait to reverse</param>
    /// <typeparam name="T">type you want to reverse</typeparam>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe T Reverse<T>(T value) where T : unmanaged
#if NET9_0_OR_GREATER
        ,
        // dotnet 8 not support by ref 
        allows ref struct
#endif
    {
        ReverseNoCheck(&value, sizeof(T));
        return value;
    }

    /// <summary>
    ///     reverses the endianness of an unmanaged type(value).
    /// </summary>
    /// <param name="value">target position you want to reverse</param>
    /// <param name="length">The size of the element that this pointer points to</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe void Reverse(void* value, int length)
    {
        ReverseNoCheck(&value, length);
    }


    /// <summary>
    ///     converts the endianness of an unmanaged type(value) from one to another.
    ///     it cannot to make sure struct members are all in the same endian.it just reverses byte-endianness of the whole
    ///     struct.
    /// </summary>
    /// <param name="value">target value</param>
    /// <param name="from">source endian,can use local</param>
    /// <param name="to">target endian,can use local</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe T Convert<T>(T value, Endianness from, Endianness to) where T : unmanaged
#if NET9_0_OR_GREATER
        ,
        // dotnet 8 not support by ref 
        allows ref struct
#endif
    {
        // differ, reverse
        if (from != to) ReverseNoCheck(&value, sizeof(T));

        return value;
    }

    #endregion


    #region Multiple

    /// <summary>
    ///     reverses the endianness of multiple unmanaged type(values).
    /// </summary>
    /// <param name="values">target enumerable wait to reverse</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe void ReverseMany<T>(Span<T> values) where T : unmanaged
    {
        switch (sizeof(T))
        {
            case 1:
                return;
            case 2:
                var case2 = MemoryMarshal.Cast<T, short>(values);
                BinaryPrimitives.ReverseEndianness(case2, case2);
                return;
            case 4:
                var case4 = MemoryMarshal.Cast<T, int>(values);
                BinaryPrimitives.ReverseEndianness(case4, case4);
                return;
            case 8:
                var case8 = MemoryMarshal.Cast<T, long>(values);
                BinaryPrimitives.ReverseEndianness(case8, case8);
                return;
            case 16:
                var case16 = MemoryMarshal.Cast<T, Int128>(values);
                BinaryPrimitives.ReverseEndianness(case16, case16);
                return;
            default:
                MemoryMarshal.Cast<T, byte>(values).Reverse();
                values.Reverse();
                return;
        }
    }

    /// <summary>
    ///     reverses the endianness of multiple unmanaged type(values).
    /// </summary>
    /// <param name="target">target position to reverse endianness</param>
    /// <param name="elementSize">The size of the element that this pointer points to</param>
    /// <param name="elementCount">The number of elements pointed to by this pointer</param>
    public static unsafe void ReverseMany(void* target, int elementSize, int elementCount)
    {
       
        switch (elementSize)
        {
            case 1:
                return;
            case sizeof(short):
                ReverseMany(new Span<short>(target, elementCount));
                return;
            case sizeof(int):
                ReverseMany(new Span<int>(target, elementCount));
                return;
            case sizeof(long):
                ReverseMany(new Span<long>(target, elementCount));
                return;
            case sizeof(long) * 2:
                ReverseMany(new Span<Int128>(target, elementCount));
                return;
            // How can I get standard library-level performance,HOW!
            default:
                var byteLenght = elementCount * elementSize;
                var data = new Span<byte>(target, byteLenght);
                // reverse bytes
                data.Reverse();
                // reverse index
                Span<byte> tempBuffer = stackalloc byte[elementSize];
                var (leftPos, rightPos) = (0, elementCount - 1);
                while (leftPos < rightPos)
                {
                    var left = data.Slice(leftPos * elementSize, elementSize);
                    var right = data.Slice(rightPos*elementSize,elementSize);
                    left.CopyTo(tempBuffer);
                    right.CopyTo(left);
                    tempBuffer.CopyTo(right);
                    leftPos++;
                    rightPos--;
                }
                return;
        }
    }

    /// <summary>
    ///     converts the endianness of multiple unmanaged type(value) from one to another.
    /// </summary>
    /// <param name="target">target position to reverse endianness</param>
    /// <param name="from">source endian</param>
    /// <param name="to">target endian</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe void ConvertMany<T>(Span<T> target, Endianness from, Endianness to) where T : unmanaged
    {
        // same,do nothing
        if (from == to) return;
        // differ, reverse
        ReverseMany(target);
    }

    /// <summary>
    ///     converts the endianness of multiple unmanaged type(value) from one to another.
    /// </summary>
    /// <param name="target">target position to reverse endianness</param>
    /// <param name="elementSize">The size of the element that this pointer points to</param>
    /// <param name="elementCount">The number of elements pointed to by this pointer</param>
    /// <param name="from">source endian</param>
    /// <param name="to">target endian</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe void ConvertMany(void* target, int elementSize, int elementCount, Endianness from,
        Endianness to)
    {
        // same,do nothing
        if (from == to) return;
        // differ, reverse
        ReverseMany(target, elementSize, elementCount);
    }

    #endregion
}

/// <summary>
///     endianness type definition
/// </summary>
public enum Endianness
{
    /// <summary>
    ///     little-endian byte order
    /// </summary>
    Little,

    /// <summary>
    ///     big-endian byte order
    /// </summary>
    Big
}

/// Extension of
/// <see cref="Endianness" />
/// ,provide
/// <c>Local</c>
public static class EndiannessExtension
{
    /// <summary>
    ///     Same as <see cref="EndiannessExtension.extension(Endianness).Local">Local</see>,Just to support C# 13 and less
    /// </summary>
    public static readonly Endianness LocalEndianness =
        BitConverter.IsLittleEndian ? Endianness.Little : Endianness.Big;

    /// <summary />
    extension(Endianness obj)
    {
        /// <summary>
        ///     local machine byte order
        ///     same as BitConverter.IsLittleEndian
        /// </summary>
        public static Endianness Local => LocalEndianness;
    }
}