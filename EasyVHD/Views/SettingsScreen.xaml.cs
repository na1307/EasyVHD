using EasyVhd.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.
namespace EasyVhd.Views;

public sealed partial class SettingsScreen {
    private readonly SettingsScreenViewModel vm = new();

    public SettingsScreen() => InitializeComponent();
}
