using System.Runtime.InteropServices;

namespace BinaryKit_Test;

public record struct UnmanagedStruct
{
    public int A;
    public float B;
    public short C;
}

[StructLayout(LayoutKind.Explicit)]
public record struct UnionStruct
{
    [FieldOffset(0)] public int A;
    [FieldOffset(0)] public float B;
    [FieldOffset(0)] public long C;
}