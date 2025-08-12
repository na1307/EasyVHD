using System.Runtime.InteropServices;

namespace EasyVhd.Internals;

[StructLayout(LayoutKind.Sequential)]
internal struct VirtualStorageType {
    public VirtualStorageTypeDevice DeviceId;
    public Guid VendorId;
}
