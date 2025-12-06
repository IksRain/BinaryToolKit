using System.Buffers.Binary;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Iks.BinaryToolkit;

/// <summary>
/// provides methods to process endianness of unmanaged types,including int,float.
/// </summary>
public static class EndianToolkit
{
    /// <summary>
    /// reverses the endianness of an unmanaged type(value).
    /// </summary>
    /// <param name="value">value wait to reverse</param>
    /// <typeparam name="T">type you want to reverse</typeparam>
    public static unsafe void Reverse<T>(scoped ref T value) where T : unmanaged
#if NET9_0_OR_GREATER
        ,
        // dotnet 8 not support by ref 
        allows ref struct
#endif
    {
        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
        {
            throw new ArgumentException(ErrorMessage.Not_Supported_Type_With_Pointer, nameof(T));
        }

        fixed (T* ptr = &value)
        {
            ReverseNoCheck(ptr);
        }
    }

    /// <summary>
    /// reverses the endianness of an unmanaged type(value).
    /// </summary>
    /// <param name="value">target position you want to reverse</param>
    public static unsafe void Reverse<T>(T* value) where T : unmanaged
#if NET9_0_OR_GREATER
        ,
        // dotnet 8 not support by ref 
        allows ref struct
#endif
    {
        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
        {
            throw new ArgumentException(ErrorMessage.Not_Supported_Type_With_Pointer, nameof(T));
        }

        ReverseNoCheck(value);
    }

    private static unsafe void ReverseNoCheck<T>(T* value) where T : unmanaged
#if NET9_0_OR_GREATER
        ,
        // dotnet 8 not support by ref 
        allows ref struct
#endif
    {
        switch (sizeof(T))
        {
            case 1:
                return;
            case 2:
                var temp2 = BinaryPrimitives.ReverseEndianness(*(short*)value);
                *(short*)value = temp2;
                break;
            case 4:
                var temp4 = BinaryPrimitives.ReverseEndianness(*(int*)value);
                *(int*)value = temp4;
                break;
            case 8:
                var temp8 = BinaryPrimitives.ReverseEndianness(*(long*)value);
                *(long*)value = temp8;
                break;
#if NET5_0_OR_GREATER
            case 16:
                var temp16 = BinaryPrimitives.ReverseEndianness(*(Int128*)value);
                *(Int128*)value = temp16;
                break;
#endif
            default:
                new Span<byte>(value, sizeof(T)).Reverse();
                break;
        }
    }

    /// <summary>
    /// converts the endianness of an unmanaged type(value) from one to another.
    /// it cannot to make sure struct members are all in the same endian.it just reverses byte-endianness of the whole struct.
    /// </summary>
    /// <param name="from">source endian,can use local</param>
    /// <param name="to">target endian,can use local</param>
    public static void Convert<T>(scoped ref T value, Endianness from, Endianness to) where T : unmanaged
#if NET9_0_OR_GREATER
        ,
        // dotnet 8 not support by ref 
        allows ref struct
#endif
    {
        //process local
        if (from is Endianness.Local) from = BitConverter.IsLittleEndian ? Endianness.Little : Endianness.Big;
        if (to is Endianness.Local) to = BitConverter.IsLittleEndian ? Endianness.Little : Endianness.Big;
        // same,do nothing
        if (from == to) return;
        // differ, reverse
        Reverse(ref value);
    }


    #region Multiple

    /// <summary>
    /// reverses the endianness of multiple unmanaged type(values).
    /// </summary>
    /// <param name="values">target enumerable wait to reverse</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe void ReverseMany<T>(Span<T> values) where T : unmanaged
    {
        ReverseMany((T*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(values)), values.Length);
    }

    /// <summary>
    /// reverses the endianness of multiple unmanaged type(values).
    /// </summary>
    /// <param name="target">target position to reverse endianness</param>
    /// <param name="lenght">the number of ptr field</param>
    public static unsafe void ReverseMany<T>(T* target, int lenght) where T : unmanaged
#if NET9_0_OR_GREATER
        ,
        // dotnet 8 not support by ref 
        allows ref struct
#endif
    {
        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
        {
            throw new ArgumentException(ErrorMessage.Not_Supported_Type_With_Pointer, nameof(T));
        }

        while (lenght-- > 0)
        {
            ReverseNoCheck(target);
            target++;
        }
    }

    /// <summary>
    /// converts the endianness of multiple unmanaged type(value) from one to another.
    /// </summary>
    /// <param name="target">target position to reverse endianness</param>
    /// <param name="lenght">the number of ptr field</param>
    /// <param name="from">source endian,can use local</param>
    /// <param name="to">target endian,can use local</param>
    public static unsafe void ConvertMany<T>(T* target, int lenght, Endianness from, Endianness to) where T : unmanaged
#if NET9_0_OR_GREATER
        ,
        // dotnet 8 not support by ref 
        allows ref struct
#endif
    {
        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
        {
            throw new ArgumentException(ErrorMessage.Not_Supported_Type_With_Pointer, nameof(T));
        }

        //process local
        if (from is Endianness.Local) from = BitConverter.IsLittleEndian ? Endianness.Little : Endianness.Big;
        if (to is Endianness.Local) to = BitConverter.IsLittleEndian ? Endianness.Little : Endianness.Big;
        // same,do nothing
        if (from == to) return;
        // differ, reverse
        while (lenght-- > 0)
        {
            ReverseNoCheck(target);
            target++;
        }
    }

    /// <summary>
    /// converts the endianness of multiple unmanaged type(value) from one to another.
    /// </summary>
    /// <param name="target">target position to reverse endianness</param>
    /// <param name="from">source endian,can use local</param>
    /// <param name="to">target endian,can use local</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe void ConvertMany<T>(Span<T> target, Endianness from, Endianness to) where T : unmanaged
    {
        fixed (T* ptr = &MemoryMarshal.GetReference(target))
        {
            ConvertMany(ptr, target.Length, from, to);
        }
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
    Big,

    /// <summary>
    /// local machine byte order
    /// eq as BitConverter.IsLittleEndian
    /// </summary>
    Local
}