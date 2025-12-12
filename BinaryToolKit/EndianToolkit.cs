using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Iks.BinaryToolkit;

/// <summary>
/// provides methods to process endianness of unmanaged types,including int,float.
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
    /// reverses the endianness of an unmanaged type(value).
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
    /// reverses the endianness of an unmanaged type(value).
    /// </summary>
    /// <param name="value">target position you want to reverse</param>
    /// <param name="length">The size of the element that this pointer points to</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe void Reverse(void* value, int length)
    {
        ReverseNoCheck(&value, length);
    }


    /// <summary>
    /// converts the endianness of an unmanaged type(value) from one to another.
    /// it cannot to make sure struct members are all in the same endian.it just reverses byte-endianness of the whole struct.
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
        if (from != to)
        {
            ReverseNoCheck(&value, sizeof(T));
        }

        return value;
    }

    #endregion


    #region Multiple

    /// <summary>
    /// reverses the endianness of multiple unmanaged type(values).
    /// </summary>
    /// <param name="values">target enumerable wait to reverse</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe void ReverseMany<T>(Span<T> values) where T : unmanaged
    {
        fixed (T* ptr = &MemoryMarshal.GetReference(values))
        {
            ReverseMany(ptr, sizeof(T), values.Length);
        }
    }

    /// <summary>
    /// reverses the endianness of multiple unmanaged type(values).
    /// </summary>
    /// <param name="target">target position to reverse endianness</param>
    /// <param name="elementSize">The size of the element that this pointer points to</param>
    /// <param name="elementCount">The number of elements pointed to by this pointer</param>
    public static unsafe void ReverseMany(void* target, int elementSize, int elementCount)
    {
        // do not reverse
        if (elementSize == 1) return;
        var targetPtr = (byte*)target;
        // do reverse
        while (elementCount-- > 0)
        {
            ReverseNoCheck(targetPtr, elementSize);
            targetPtr += elementSize;
        }
    }

    /// <summary>
    /// converts the endianness of multiple unmanaged type(value) from one to another.
    /// </summary>
    /// <param name="target">target position to reverse endianness</param>
    /// <param name="from">source endian</param>
    /// <param name="to">target endian</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe void ConvertMany<T>(Span<T> target, Endianness from, Endianness to) where T : unmanaged
    {
        fixed (T* ptr = &MemoryMarshal.GetReference(target))
        {
            ConvertMany(ptr, sizeof(T), target.Length, from, to);
        }
    }

    /// <summary>
    /// converts the endianness of multiple unmanaged type(value) from one to another.
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
/// endianness type definition
/// </summary>
public enum Endianness
{
    /// <summary>
    /// little-endian byte order
    /// </summary>
    Little,

    /// <summary>
    /// big-endian byte order
    /// </summary>
    Big
}

public static class EndiannessExtension
{
    private static readonly Endianness LocalEndianness =
        BitConverter.IsLittleEndian ? Endianness.Little : Endianness.Big;

    extension(Endianness obj)
    {
        /// <summary>
        /// local machine byte order
        /// same as BitConverter.IsLittleEndian
        /// </summary>
        public static Endianness Local => LocalEndianness;
    }
}