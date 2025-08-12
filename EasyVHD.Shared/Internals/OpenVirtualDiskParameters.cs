using System.Runtime.InteropServices;

namespace EasyVhd.Internals;

[StructLayout(LayoutKind.Sequential)]
internal struct OpenVirtualDiskParameters() {
    public readonly int Version = 1;
    public uint RWDepth;
}
