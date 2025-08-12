using CommunityToolkit.Mvvm.Input;
using EasyVhd.Models;
using EasyVhd.Views;

namespace EasyVhd.ViewModels;

public sealed partial class SelectVhdScreenViewModel(Screen previousScreen) : SubScreenViewModel(previousScreen) {
    [RelayCommand]
    private async Task SelectVhdFile(SelectVhdScreen screen) {
        if (await Run(screen, () => Task.CompletedTask)) {
            Status.SelectedVhdFile = VhdPath;
            Previous();
        }
    }
}
