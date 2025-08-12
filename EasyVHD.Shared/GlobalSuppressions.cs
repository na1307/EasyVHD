// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Design", "CA1028:열거형 스토리지는 Int32여야 합니다", Justification = "<보류 중>", Scope = "type", Target = "~T:EasyVhd.Internals.VirtualStorageTypeDevice")]
[assembly: SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1214:Readonly fields should appear before non-readonly fields", Justification = "<보류 중>", Scope = "member", Target = "~F:EasyVhd.Internals.CreateVirtualDiskParameters.BlockSizeInBytes")]
[assembly: SuppressMessage("Blocker Bug", "S3869:\"SafeHandle.DangerousGetHandle\" should not be called", Justification = "<보류 중>", Scope = "member", Target = "~M:EasyVhd.VhdFunctions.CreateVirtualDiskAsync(System.String,System.UInt64,EasyVhd.VhdType,System.String,System.Threading.CancellationToken)~System.Threading.Tasks.Task{EasyVhd.SafeVirtualDiskHandle}")]
[assembly: SuppressMessage("Reliability", "CA2000:범위를 벗어나기 전에 개체를 삭제하십시오.", Justification = "<보류 중>", Scope = "member", Target = "~M:EasyVhd.VhdFunctions.CreateVirtualDiskAsync(System.String,System.UInt64,EasyVhd.VhdType,System.String,System.Threading.CancellationToken)~System.Threading.Tasks.Task{EasyVhd.SafeVirtualDiskHandle}")]
[assembly: SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:Field names should not contain underscore", Justification = "<보류 중>", Scope = "member", Target = "~T:EasyVhd.VhdFunctions")]
