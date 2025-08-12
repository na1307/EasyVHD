namespace EasyVhd.Internals;

[Flags]
internal enum CreateVirtualDiskFlag {
    None = 0x0,
    FullPhysicalAllocation = 0x1,
    PreventWritesToSourceDisk = 0x2,
    DoNotCopyMetadataFromParent = 0x4,
    CreateBackingStorage = 0x8,
    UseChangeTrackingSourceLimit = 0x10,
    PreserveParentChangeTrackingState = 0x20,
    VhdSetUseOriginalBackingStorage = 0x40,
    SparseFile = 0x80,
    PmemCompatible = 0x100,
    SupportCompressedVolumes = 0x200,
    SupportSparseFilesAnyFs = 0x400
}
