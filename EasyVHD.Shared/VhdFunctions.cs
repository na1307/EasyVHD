using EasyVhd.Internals;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace EasyVhd;

public static class VhdFunctions {
    // ReSharper disable InconsistentNaming
    private const int ERROR_IO_PENDING = 997;
    private const int ERROR_OPERATION_ABORTED = 995;
    // ReSharper restore InconsistentNaming

    // ReSharper disable AccessToModifiedClosure
    // ReSharper disable AccessToDisposedClosure
    // ReSharper disable ConditionIsAlwaysTrueOrFalse
    public static unsafe Task<SafeVirtualDiskHandle> CreateVirtualDiskAsync(
        string path,
        ulong bytes,
        VhdType type,
        string? sourceOrParent,
        CancellationToken cancellationToken = default) {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(bytes, 70368744177664UL);
        ArgumentOutOfRangeException.ThrowIfLessThan(bytes, 3145728UL);

        var format = GetFormatFromPath(path);

        if (type == VhdType.Differencing) {
            if (string.IsNullOrWhiteSpace(sourceOrParent)) {
                throw new ArgumentException("차이점 보관 VHD를 만드려 했지만 원본 VHD 경로가 지정되지 않았습니다.", nameof(sourceOrParent));
            }

            if (format != GetFormatFromPath(sourceOrParent)) {
                throw new ArgumentException("원본 VHD와 차이점 보관 VHD의 형식은 같아야 합니다.");
            }
        }

        if (bytes % 512 != 0) {
            throw new ArgumentException("크기는 512의 배수여야 합니다.", nameof(bytes));
        }

        VirtualStorageType vst = new() {
            DeviceId = format switch {
                VhdFormat.Vhd => VirtualStorageTypeDevice.Vhd,
                VhdFormat.Vhdx => VirtualStorageTypeDevice.Vhdx,
                _ => throw new InvalidOperationException()
            },
            VendorId = Constants.VendorMicrosoft
        };

        CreateVirtualDiskParameters cvdp = new() {
            UniqueId = Guid.Empty,
            MaximumSize = bytes,
            ParentPath = type == VhdType.Differencing ? sourceOrParent : null,
            SourcePath = type != VhdType.Differencing ? sourceOrParent : null
        };

        TaskCompletionSource<SafeVirtualDiskHandle> tcs = new(TaskCreationOptions.RunContinuationsAsynchronously);
        EventWaitHandle ewh = new(false, EventResetMode.ManualReset);

        Overlapped o = new() {
            EventHandleIntPtr = ewh.SafeWaitHandle.DangerousGetHandle()
        };

        var no = o.Pack(null, null);
        var flag = type == VhdType.Fixed ? CreateVirtualDiskFlag.FullPhysicalAllocation : CreateVirtualDiskFlag.None;
        var hr = NativeMethods.CreateVirtualDisk(in vst, path, VirtualDiskAccessMask.All, null, flag, 0, in cvdp, no, out var handle);

        if (hr != ERROR_IO_PENDING) {
            Overlapped.Free(no);
            ewh.Dispose();

            throw new Win32Exception((int)hr);
        }

        var sharedHandle = handle;
        CancellationTokenRegistration ctr = default;

        if (cancellationToken.CanBeCanceled) {
            ctr = cancellationToken.Register(() => {
                var owned = Interlocked.Exchange(ref sharedHandle, null);

                if (owned is null) {
                    // 이미 다른 경로(성공/오류)에서 처리되었음
                    return;
                }

                try {
                    NativeMethods.CancelIoEx(owned.DangerousGetHandle(), no);
                    tcs.TrySetCanceled(cancellationToken);
                } finally {
                    owned.Dispose();
                }
            });
        }

        var rwh = ThreadPool.RegisterWaitForSingleObject(ewh, (_, _) => {
            var owned = Interlocked.Exchange(ref sharedHandle, null);

            if (owned is null) {
                // 이미 취소되었거나 다른 경로가 처리함
                return;
            }

            try {
                if (NativeMethods.GetOverlappedResult(owned.DangerousGetHandle(), no, out _, true)) {
                    tcs.TrySetResult(owned);
                } else {
                    var error = Marshal.GetLastPInvokeError();

                    if (error == ERROR_OPERATION_ABORTED) {
                        tcs.TrySetCanceled(cancellationToken);
                    } else {
                        tcs.TrySetException(new Win32Exception(error));
                    }

                    owned.Dispose();
                }
            } catch (Exception ex) {
                tcs.TrySetException(ex);
                owned.Dispose();
            }
        }, null, -1, true);

        tcs.Task.GetAwaiter().OnCompleted(() => {
            rwh.Unregister(null);
            ctr.Dispose();
            Overlapped.Free(no);
            ewh.Dispose();

            // o를 여기서 참조함으로써 o가 OnCompleted까지 살아있게 함 (GC 보호)
            GC.KeepAlive(o);
        });

        return tcs.Task;
    }

    // ReSharper restore ConditionIsAlwaysTrueOrFalse
    // ReSharper restore AccessToDisposedClosure
    // ReSharper restore AccessToModifiedClosure
    public static async Task AttachVirtualDiskAsync(string path, bool readOnly) {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        VirtualStorageType vst = new() {
            DeviceId = GetFormatFromPath(path) switch {
                VhdFormat.Vhd => VirtualStorageTypeDevice.Vhd,
                VhdFormat.Vhdx => VirtualStorageTypeDevice.Vhdx,
                _ => throw new InvalidOperationException()
            },
            VendorId = Constants.VendorMicrosoft
        };

        uint hr;
        SafeVirtualDiskHandle handle;

        unsafe {
            hr = NativeMethods.OpenVirtualDisk(in vst, path, VirtualDiskAccessMask.All, OpenVirtualDiskFlag.None, null, out handle);
        }

        using (handle) {
            if (hr != 0) {
                throw new Win32Exception((int)hr);
            }

            await AttachVirtualDiskAsync(handle, readOnly);
        }
    }

    public static async Task AttachVirtualDiskAsync(SafeVirtualDiskHandle handle, bool readOnly) {
        ArgumentNullException.ThrowIfNull(handle);

        var flags = AttachVirtualDiskFlag.PermanentLifetime;

        if (readOnly) {
            flags |= AttachVirtualDiskFlag.ReadOnly;
        }

        var avd = await Task.Run(() => {
            unsafe {
                return NativeMethods.AttachVirtualDisk(handle, null, flags);
            }
        });

        if (avd != 0) {
            throw new Win32Exception((int)avd);
        }
    }

    public static async Task DetachVirtualDiskAsync(string path) {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        VirtualStorageType vst = new() {
            DeviceId = GetFormatFromPath(path) switch {
                VhdFormat.Vhd => VirtualStorageTypeDevice.Vhd,
                VhdFormat.Vhdx => VirtualStorageTypeDevice.Vhdx,
                _ => throw new InvalidOperationException()
            },
            VendorId = Constants.VendorMicrosoft
        };

        uint hr;
        SafeVirtualDiskHandle handle;

        unsafe {
            hr = NativeMethods.OpenVirtualDisk(in vst, path, VirtualDiskAccessMask.All, OpenVirtualDiskFlag.None, null, out handle);
        }

        using (handle) {
            if (hr != 0) {
                throw new Win32Exception((int)hr);
            }

            await DetachVirtualDiskAsync(handle);
        }
    }

    public static async Task DetachVirtualDiskAsync(SafeVirtualDiskHandle handle) {
        ArgumentNullException.ThrowIfNull(handle);

        var hr = await Task.Run(() => NativeMethods.DetachVirtualDisk(handle));

        if (hr != 0) {
            throw new Win32Exception((int)hr);
        }
    }

    private static VhdFormat GetFormatFromPath(string path) {
        var extension = Path.GetExtension(path).TrimStart('.');

        if (extension.Equals("vhd", StringComparison.OrdinalIgnoreCase)) {
            return VhdFormat.Vhd;
        }

        if (extension.Equals("vhdx", StringComparison.OrdinalIgnoreCase)) {
            return VhdFormat.Vhdx;
        }

        throw new ArgumentException("확장자가 .vhd나 .vhdx가 아님");
    }
}
