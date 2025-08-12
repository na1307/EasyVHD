using CommunityToolkit.Mvvm.DependencyInjection;
using EasyVhd.ViewModels;
using EasyVhd.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using System.Runtime.InteropServices;
using WinUIEx;
using UnhandledExceptionEventArgs = Microsoft.UI.Xaml.UnhandledExceptionEventArgs;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.
namespace EasyVhd;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public sealed partial class App {
    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App() {
        UnhandledException += App_UnhandledException;
        ServiceCollection sc = new();
        sc.AddSingleton<MainWindowViewModel>();
        Ioc.Default.ConfigureServices(sc.BuildServiceProvider());
        InitializeComponent();
    }

    public static new App Current => (App)Application.Current;

    public Window? Window { get; private set; }

    /// <summary>
    /// Invoked when the application is launched.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override void OnLaunched(LaunchActivatedEventArgs args) {
        Window = new MainWindow(Ioc.Default.GetRequiredService<MainWindowViewModel>());
        Window.SetWindowSize(960, 640);
        Window.SetIsResizable(false);
        Window.SetIsMaximizable(false);
        Window.CenterOnScreen();
        Window.Activate();
    }

    private void App_UnhandledException(object sender, UnhandledExceptionEventArgs e) {
        _ = MessageBoxW(IntPtr.Zero, e.Exception.ToString(), null, 16);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
        static extern int MessageBoxW(IntPtr hWnd, [MarshalAs(UnmanagedType.LPWStr)] string? text, [MarshalAs(UnmanagedType.LPWStr)] string? caption, uint type);
    }
}
