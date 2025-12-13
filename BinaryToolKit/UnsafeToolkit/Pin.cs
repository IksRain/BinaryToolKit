using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Iks.BinaryToolKit.Message;

namespace Iks.BinaryToolkit.UnsafeToolkit;

/// <summary>
///     Generics Supported and <c>Auto-Release</c> <see cref="GCHandle" /> of <see cref="GCHandleType.Pinned" />.
/// </summary>
/// <param name="objectNeededPinning">An object which you want to pin for sometime</param>
#if NET10_0_OR_GREATER
[Obsolete(ObsoleteMessage.Use_PinnedGCHandle_From_Standard, false)]
#endif
[DebuggerDisplay("Address = {Address}, Target = {Target}")]
public struct Pin<TPinned>(TPinned objectNeededPinning) : IDisposable, IEquatable<Pin<TPinned>> where TPinned : class
{
    private GCHandle _handle =
        GCHandle.Alloc(objectNeededPinning ?? throw new ArgumentNullException(nameof(objectNeededPinning)),
            GCHandleType.Pinned);

    /// <summary>
    ///     Get pinned object
    /// </summary>
    public readonly TPinned Target => (TPinned)_handle.Target!;

    /// <summary>
    ///     Retrieves the <c>address</c> of object data in a <see cref="GCHandleType.Pinned">Pinned</see> handle.
    /// </summary>
    /// <returns>the <c>address</c> of the object</returns>
    public readonly IntPtr Address => _handle.AddrOfPinnedObject();

    /// <summary> Free the <see cref="GCHandle" /> to <c>unpin</c> it </summary>
    public void Dispose()
    {
        _handle.Free();
    }

    #region Wrapper GCHandle

    /// <summary>
    ///     Equals As <see cref="GCHandle.GetHashCode" />
    ///     Returns an identifier for the current <see cref="GCHandle" /> object.
    /// </summary>
    /// <returns>An identifier for the current <see cref="GCHandle" /> object</returns>
    public readonly override int GetHashCode()
    {
        return _handle.GetHashCode();
    }

    /// <summary>
    ///     Same as <see cref="GCHandle.Equals(GCHandle)">GCHandle.Equals</see>.
    ///     <para>Indicates whether the current instance is equal to another instance of the same type.</para>
    /// </summary>
    /// <param name="other">An instance to compare with this instance.</param>
    /// <returns>true if the current instance is equal to the other instance; otherwise, false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Equals(Pin<TPinned> other)
    {
        return _handle.Equals(other._handle);
    }

    /// <summary>
    ///     Same as <see cref="GCHandle.Equals(GCHandle)">GCHandle.Equals</see>.
    ///     <para>Indicates whether the current instance is equal to another instance of the same type.</para>
    /// </summary>
    /// <param name="other">An instance to compare with this instance.</param>
    /// <returns>true if the current instance is equal to the other instance; otherwise, false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Equals(GCHandle other)
    {
        return _handle.Equals(other);
    }

    /// <summary>
    ///     Same as <see cref="GCHandle.Equals(GCHandle)">GCHandle.Equals</see>.
    ///     <para>Indicates whether the current instance is equal to another instance of the same type.</para>
    /// </summary>
    /// <param name="other">An instance to compare with this instance.</param>
    /// <returns>true if the current instance is equal to the other instance; otherwise, false.</returns>
    public readonly override bool Equals(object? other)
    {
        return other is GCHandle obj && _handle.Equals(obj);
    }

    /// <summary>
    ///     Same As
    ///     <see cref="Equals(Pin{TPinned})">
    ///         Equals
    ///     </see>
    /// </summary>
    public static bool operator ==(Pin<TPinned> left, Pin<TPinned> right)
    {
        return left.Equals(right);
    }

    /// <summary>
    ///     Same As
    ///     <see cref="Equals(Pin&lt;TPinned&gt;)">
    ///         Equals
    ///     </see>
    /// </summary>
    public static bool operator !=(Pin<TPinned> left, Pin<TPinned> right)
    {
        return !(left == right);
    }

    /// <summary>
    ///     Display <see cref="Pin{TPinned}">Pin</see> ,including Address and Target.ToString()
    /// </summary>
    public override string ToString()
    {
        return $"Pinned Address = {Address}, Target = {Target}";
    }

    #endregion
}

/// <summary>
///     Provide <see cref="Pin{TPinned}">Pin</see> extension ,such as PinIt,
/// </summary>
#if NET10_0_OR_GREATER
[Obsolete(ObsoleteMessage.Use_PinnedGCHandle_From_Standard, false)]
#endif
public static class PinExtension
{
    /// <summary />
    extension<TPinned>(TPinned obj) where TPinned : class
    {
        /// <summary>
        ///     Make a <see cref="Pin{TPinned}">Pin&lt;TPinned&gt;</see> from obj
        /// </summary>
        /// <returns></returns>
        public Pin<TPinned> AsPinned()
        {
            return new Pin<TPinned>(obj);
        }
    }
}