using System.Runtime.InteropServices;

namespace EasyVhd.Internals;

[StructLayout(LayoutKind.Sequential)]
internal struct CreateVirtualDiskParameters() {
    public readonly int Version = 1;
    public Guid UniqueId;
    public ulong MaximumSize;
    public readonly uint BlockSizeInBytes;
    public readonly uint SectorSizeInBytes;

    [MarshalAs(UnmanagedType.LPWStr)]
    public string? ParentPath;

    [MarshalAs(UnmanagedType.LPWStr)]
    public string? SourcePath;
}
