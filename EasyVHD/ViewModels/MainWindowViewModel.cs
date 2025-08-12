using CommunityToolkit.Mvvm.ComponentModel;
using EasyVhd.Views;
using Microsoft.UI.Xaml.Controls;

namespace EasyVhd.ViewModels;

public sealed partial class MainWindowViewModel : ViewModel {
    [ObservableProperty]
    public partial Screen CurrentScreen { get; set; } = null!;

    public void NV(NavigationView sender, NavigationViewSelectionChangedEventArgs args) {
        if (!args.IsSettingsSelected) {
            var si = (NavigationViewItem)args.SelectedItem;
            var typeName = $"EasyVhd.Views.{si.Tag}Screen";
            var type = Type.GetType(typeName)!;
            CurrentScreen = (Screen)Activator.CreateInstance(type)!;
        } else {
            CurrentScreen = new SettingsScreen();
        }
    }
}
