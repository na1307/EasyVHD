namespace EasyVhd.ViewModels;

public sealed partial class SettingsScreenViewModel : ViewModel {
    public string BuildNumberString => $"빌드 {BuildNumber}";
}
