using CommunityToolkit.Mvvm.Input;
using EasyVhd.Models;
using EasyVhd.Views;

namespace EasyVhd.ViewModels;

public sealed partial class DetachVhdScreenViewModel : SubScreenViewModel {
    public DetachVhdScreenViewModel(Screen previousScreen) : base(previousScreen) {
        if (Status.SelectedVhdFile is not null) {
            VhdPath = Status.SelectedVhdFile;
        }
    }

    [RelayCommand]
    private async Task DetachVhdFile(DetachVhdScreen screen) {
        if (await Run(screen, () => VhdFunctions.DetachVirtualDiskAsync(VhdPath))) {
            Previous();
        }
    }
}
