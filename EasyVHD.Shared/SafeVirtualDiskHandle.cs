using EasyVhd.Internals;
using Microsoft.Win32.SafeHandles;

namespace EasyVhd;

public sealed class SafeVirtualDiskHandle : SafeHandleZeroOrMinusOneIsInvalid {
    public SafeVirtualDiskHandle() : base(true) { }

    public SafeVirtualDiskHandle(IntPtr preexistingHandle, bool ownsHandle) : base(ownsHandle) => SetHandle(preexistingHandle);

    protected override bool ReleaseHandle() => NativeMethods.CloseHandle(handle);
}
