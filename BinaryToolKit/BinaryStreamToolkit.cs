using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Iks.BinaryToolkit;

/// <summary>
///     To replace <see cref="BinaryReader" />/<see cref="BinaryWriter" /> with new extension methods for
///     <see cref="Stream" />.
///     <para>
///         This change aims to simplify the API and <c>Generic Support</c>.
///     </para>
/// </summary>
public static class BinaryStreamToolkit
{
    /// <summary>
    ///     provides extension methods for <see cref="Stream" /> to read and write unmanaged types.
    /// </summary>
    /// <param name="stream"></param>
    extension(Stream stream)
    {
        #region single kit

        /// <summary>
        ///     read an unmanaged type from the stream.
        /// </summary>
        /// <typeparam name="T">type you want to read</typeparam>
        /// <exception cref="EndOfStreamException">stream had ended</exception>
        [SkipLocalsInit]
        public unsafe T ReadAs<T>() where T : unmanaged
#if NET9_0_OR_GREATER
            ,
            // dotnet 8 not support by ref 
            allows ref struct
#endif
        {
            if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
                throw new NotSupportedException(ErrorMessage.Not_Supported_Type_With_Pointer);
            var value = stackalloc byte[sizeof(T)];
            var count = stream.Read(new Span<byte>(&value, sizeof(T)));
            if (count != sizeof(T)) throw new EndOfStreamException(ErrorMessage.Not_Enough_Data_In_Stream);
            return *(T*)value;
        }

        /// <summary>
        ///     read an unmanaged type from the stream.
        /// </summary>
        /// <param name="target">target position in the memory of type you want to read</param>
        public unsafe void ReadAs<T>(out T target) where T : unmanaged
#if NET9_0_OR_GREATER
            ,
            // dotnet 8 not support by ref 
            allows ref struct
#endif
        {
            if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
                throw new NotSupportedException(ErrorMessage.Not_Supported_Type_With_Pointer);
            fixed (void* ptr = &target)
            {
                var count = stream.Read(new Span<byte>(ptr, sizeof(T)));
                if (count != sizeof(T)) throw new EndOfStreamException(ErrorMessage.Not_Enough_Data_In_Stream);
            }
        }

        /// <summary>
        ///  write an unmanaged type(value) to the stream.
        /// </summary>
        /// <typeparam name="T">target type</typeparam>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void WriteFrom<T>(T value) where T : unmanaged
#if NET9_0_OR_GREATER
            ,
            // dotnet 8 not support by ref 
            allows ref struct
#endif
        {
            stream.Write(new ReadOnlySpan<byte>(&value, sizeof(T)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void WriteFrom<T>(ref readonly T value) where T : unmanaged
#if NET9_0_OR_GREATER
            ,
            // dotnet 8 not support by ref 
            allows ref struct
#endif
        {
            fixed (void* ptr = &value)
            {
                stream.Write(new ReadOnlySpan<byte>(ptr, sizeof(T)));
            }
        }

        #endregion

        #region multiple kit

        /// <summary>
        ///     read multiple(<c>Span.Length</c>) value from stream
        /// </summary>
        /// <param name="targetSpan">target when load from stream</param>
        /// <typeparam name="T">type you want to read</typeparam>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void ReadManyAs<T>(Span<T> targetSpan) where T : unmanaged
        {
            fixed(T* ptrT = &MemoryMarshal.GetReference(targetSpan))
            {
                ReadManyAs(stream, ptrT, targetSpan.Length);
            }
        }

        /// <summary>
        ///     read multiple(<c>count</c>) value from stream
        /// </summary>
        /// <param name="targetPtr">target position when load from stream</param>
        /// <param name="count">the number of ptr length</param>
        /// <typeparam name="T">type you want to read</typeparam>
        public unsafe void ReadManyAs<T>(T* targetPtr, int count) where T : unmanaged
#if NET9_0_OR_GREATER
            ,
            // dotnet 8 not support by ref 
            allows ref struct
#endif
        {
            ArgumentOutOfRangeException.ThrowIfNegative(count);
            // if it has pointer,throws
            if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
                throw new NotSupportedException(ErrorMessage.Not_Supported_Type_With_Pointer);
            stream.ReadExactly(new Span<byte>(targetPtr, sizeof(T) * count));
        }

        /// <summary>
        ///     write multiple(<c>count</c>) unmanaged value to stream
        /// </summary>
        /// <param name="values">target position</param>
        /// <param name="count">the number of ptr length</param>
        public unsafe void WriteManyFrom<T>(T* values, int count) where T :
            unmanaged
#if NET9_0_OR_GREATER
            ,
            // dotnet 8 not support by ref 
            allows ref struct
#endif
        {
            stream.Write(new ReadOnlySpan<byte>(values, sizeof(T) * count));
        }

        /// <summary>
        ///     write multiple(<c>ReadOnlySpan.Length</c>) unmanaged value to stream
        /// </summary>
        /// <param name="values">target position</param>
        public unsafe void WriteManyFrom<T>(ReadOnlySpan<T> values) where T : unmanaged
        {
            fixed(T* ptrT = &MemoryMarshal.GetReference(values))
            {
                WriteManyFrom(stream,ptrT, values.Length);
            }
        }

        #endregion
    }
}