using System.Runtime.InteropServices;

namespace EasyVhd.Internals;

internal static class NativeMethods {
    [DllImport("virtdisk.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
    public static extern unsafe uint OpenVirtualDisk(
        in VirtualStorageType virtualStorageType,
        [MarshalAs(UnmanagedType.LPWStr)] string path,
        VirtualDiskAccessMask virtualDiskAccessMask,
        OpenVirtualDiskFlag flags,
        OpenVirtualDiskParameters* parameters,
        out SafeVirtualDiskHandle handle);

    [DllImport("virtdisk.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
    public static extern unsafe uint CreateVirtualDisk(
        in VirtualStorageType virtualStorageType,
        [MarshalAs(UnmanagedType.LPWStr)] string path,
        VirtualDiskAccessMask virtualDiskAccessMask,
        void* securityDescriptor,
        CreateVirtualDiskFlag flags,
        uint providerSpecificFlags,
        in CreateVirtualDiskParameters parameters,
        NativeOverlapped* overlapped,
        out SafeVirtualDiskHandle handle);

    [DllImport("virtdisk.dll", ExactSpelling = true)]
    public static extern unsafe uint AttachVirtualDisk(
        SafeVirtualDiskHandle virtualDiskHandle,
        void* securityDescriptor,
        AttachVirtualDiskFlag flags,
        uint providerSpecificFlags = 0,
        void* parameters = null,
        NativeOverlapped* overlapped = null);

    [DllImport("virtdisk.dll", ExactSpelling = true)]
    public static extern uint DetachVirtualDisk(SafeVirtualDiskHandle virtualDiskHandle, int flags = 0, uint providerSpecificFlags = 0);

    [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
    public static extern unsafe bool CancelIoEx(IntPtr file, NativeOverlapped* overlapped);

    [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern unsafe bool GetOverlappedResult(
        IntPtr file,
        NativeOverlapped* overlapped,
        out uint numberOfBytesTransferred,
        [MarshalAs(UnmanagedType.Bool)] bool wait);

    [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool CloseHandle(IntPtr hObject);
}
