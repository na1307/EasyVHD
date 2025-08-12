using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace EasyVhd;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
internal sealed class VhdPathAttribute : ValidationAttribute {
    private static readonly Regex VhdRegex = new(@"^[A-Za-z]:\\.*\.vhdx?$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public VhdPathAttribute() : this(true, true) { }

    public VhdPathAttribute(bool existing, bool required) {
        Existing = existing;
        Required = required;
    }

    public bool Existing { get; }

    public bool Required { get; }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext) {
        if (value is not string path || string.IsNullOrWhiteSpace(path)) {
            return Required ? new("VHD 경로는 비어 있지 않아야 합니다.") : ValidationResult.Success;
        }

        if (!VhdRegex.IsMatch(path)) {
            return new("지정한 경로는 올바른 VHD 혹은 VHDX 파일이 아닙니다.");
        }

        if (Existing && !File.Exists(path)) {
            return new("지정한 VHD 혹은 VHDX 파일이 존재하지 않습니다.");
        }

        return ValidationResult.Success;
    }
}
