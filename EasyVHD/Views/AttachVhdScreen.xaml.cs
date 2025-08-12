using EasyVhd.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.
namespace EasyVhd.Views;

public sealed partial class AttachVhdScreen {
    private readonly AttachVhdScreenViewModel vm;

    public AttachVhdScreen(Screen previousScreen) {
        vm = new(previousScreen);
        InitializeComponent();
    }
}
