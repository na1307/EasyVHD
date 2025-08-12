using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using EasyVhd.Models;
using EasyVhd.Views;

namespace EasyVhd.ViewModels;

public sealed partial class HomeScreenViewModel : MainScreenViewModel {
    [RelayCommand]
    private void SelectVhd(HomeScreen o) => Ioc.Default.GetRequiredService<MainWindowViewModel>().CurrentScreen = new SelectVhdScreen(o);

    [RelayCommand]
    private void CreateVhd(HomeScreen o) => Ioc.Default.GetRequiredService<MainWindowViewModel>().CurrentScreen = new CreateVhdScreen(o);

    [RelayCommand]
    private void DeselectVhd() => Status.SelectedVhdFile = null;
}
