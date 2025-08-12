using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using EasyVhd.Views;

namespace EasyVhd.ViewModels;

public sealed partial class ToolsScreenViewModel : MainScreenViewModel {
    [RelayCommand]
    private void AttachVhd(ToolsScreen o) => Ioc.Default.GetRequiredService<MainWindowViewModel>().CurrentScreen = new AttachVhdScreen(o);

    [RelayCommand]
    private void DetachVhd(ToolsScreen o) => Ioc.Default.GetRequiredService<MainWindowViewModel>().CurrentScreen = new DetachVhdScreen(o);

    [RelayCommand]
    private void ExpandVhd(ToolsScreen o) { }

    [RelayCommand]
    private void MergeVhd(ToolsScreen o) { }
}
