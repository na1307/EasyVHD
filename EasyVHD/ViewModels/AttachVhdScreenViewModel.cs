using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasyVhd.Models;
using EasyVhd.Views;

namespace EasyVhd.ViewModels;

public sealed partial class AttachVhdScreenViewModel : SubScreenViewModel {
    public AttachVhdScreenViewModel(Screen previousScreen) : base(previousScreen) {
        if (Status.SelectedVhdFile is not null) {
            VhdPath = Status.SelectedVhdFile;
        }
    }

    [ObservableProperty]
    public partial bool ReadOnly { get; set; } = false;

    [RelayCommand]
    private async Task AttachVhdFile(AttachVhdScreen screen) {
        if (await Run(screen, () => VhdFunctions.AttachVirtualDiskAsync(VhdPath, ReadOnly))) {
            Previous();
        }
    }
}
