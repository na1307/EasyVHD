using CommunityToolkit.Mvvm.Input;

namespace EasyVhd.ViewModels;

public sealed partial class BackupRestoreScreenViewModel : MainScreenViewModel {
    [RelayCommand]
    private void AddInstance() { }

    [RelayCommand]
    private void RemoveInstance() { }

    [RelayCommand]
    private void FullBackup() { }

    [RelayCommand]
    private void FullRestore() { }

    [RelayCommand]
    private void Asdf1() { }

    [RelayCommand]
    private void Asdf2() { }
}
