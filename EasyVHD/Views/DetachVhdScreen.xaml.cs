using EasyVhd.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.
namespace EasyVhd.Views;

public sealed partial class DetachVhdScreen {
    private readonly DetachVhdScreenViewModel vm;

    public DetachVhdScreen(Screen previousScreen) {
        vm = new(previousScreen);
        InitializeComponent();
    }
}
