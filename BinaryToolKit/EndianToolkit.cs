using System.Buffers.Binary;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Iks.BinaryToolkit;

/// <summary>
/// provides methods to process endianness of unmanaged types,including int,float.
/// </summary>
public static class EndianToolkit
{
    /// <summary>
    /// reverses the endianness of an unmanaged type(value).
    /// </summary>
    /// <param name="value">value wait to reversing</param>
    /// <typeparam name="T">type you want to reverse</typeparam>
    public static unsafe void Reverse<T>(scoped ref T value) where T : unmanaged
    {
        
        fixed (void* ptr = &value)
        {
            switch (sizeof(T))
            {
                case 1:
                    return;
                case 2:
                    var temp2 = BinaryPrimitives.ReverseEndianness(*(short*)ptr);
                    *(short*)ptr = temp2;
                    break;
                case 4:
                    var temp4 = BinaryPrimitives.ReverseEndianness(*(int*)ptr);
                    *(int*)ptr = temp4;
                    break;
                case 8:
                    var temp8 = BinaryPrimitives.ReverseEndianness(*(long*)ptr);
                    *(long*)ptr = temp8;
                    break;
#if NET5_0_OR_GREATER
                case 16:
                    var temp16 = BinaryPrimitives.ReverseEndianness(*(Int128*)ptr);
                    *(Int128*)ptr = temp16;
                    break;
#endif
                default:
                    new Span<byte>(ptr, sizeof(T)).Reverse();
                    break;
            }
        }
    }

    /// <summary>
    /// converts the endianness of an unmanaged type(value) from one to another.
    /// it cannot to make sure struct members are all in the same endian.it just reverses byte-endianness of the whole struct.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="from">source endian,can use local</param>
    /// <param name="to">target endian,can use local</param>
    public static void Convert<T>(scoped ref T value, Endianness from, Endianness to) where T : unmanaged
    {
        //process local
        if(from is Endianness.Local)from = BitConverter.IsLittleEndian? Endianness.Little : Endianness.Big;
        if(to is Endianness.Local)to = BitConverter.IsLittleEndian? Endianness.Little : Endianness.Big;
        // same,do nothing
        if (from == to) return;
        // differ, reverse
        Reverse(ref value);
        return;
    }


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