using System.Runtime.CompilerServices;

namespace Iks.BinaryToolkit;

///   <summary>
///   To replace <see cref="BinaryReader"/>/<see cref="BinaryWriter"/> with new extension methods for <see cref="Stream"/>.
///   <para>
///   This change aims to simplify the API and <c>Generic Support</c>.
///   </para>
///   </summary>
public static class BinaryStreamToolkit
{
    /// <summary>
    /// provides extension methods for <see cref="Stream"/> to read and write unmanaged types.
    /// </summary>
    /// <param name="stream"></param>
    extension(Stream stream)
    {
        /// <summary>
        /// read an unmanaged type from the stream.
        /// </summary>
        /// <typeparam name="T">type you want to read</typeparam>
        /// <exception cref="EndOfStreamException">stream had ended</exception>
        [SkipLocalsInit]
        public unsafe T ReadAs<T>() where T : unmanaged
        {
            Unsafe.SkipInit(out T value);
            var count = stream.Read(new Span<byte>(&value, sizeof(T)));
            if (count != sizeof(T)) throw new EndOfStreamException(ErrorMessage.NotEnoughDataInStream);
            return value;
        }
        /// <summary>
        /// write an unmanaged type(value) to the stream.
        /// </summary>
        /// <typeparam name="T">target type</typeparam>
        public unsafe void WriteFrom<T>(scoped in T value) where T : unmanaged
        {
            fixed (void* ptr = &value)
            {
                stream.Write(new ReadOnlySpan<byte>(ptr, sizeof(T)));
            }
        }
    }
}