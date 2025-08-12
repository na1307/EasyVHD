using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;
using System.Diagnostics;
using System.Runtime.InteropServices;
using WinRT;

namespace EasyVhd;

internal static class Program {
    [STAThread]
    private static void Main(string[] args) {
        ComWrappersSupport.InitializeComWrappers();

        if (!DecideRedirection()) {
            Application.Start(p => {
                SynchronizationContext.SetSynchronizationContext(new DispatcherQueueSynchronizationContext(DispatcherQueue.GetForCurrentThread()));
                _ = new App();
            });
        }
    }

    private static bool DecideRedirection() {
        var isRedirect = false;
        var args = AppInstance.GetCurrent().GetActivatedEventArgs();
        var keyInstance = AppInstance.FindOrRegisterForKey("CDCEAA70-7A23-45E0-A3B8-812248E6A33B");

        if (keyInstance.IsCurrent) {
            keyInstance.Activated += OnActivated;
        } else {
            isRedirect = true;
            RedirectActivationTo(args, keyInstance);
        }

        return isRedirect;
    }

    private static void RedirectActivationTo(AppActivationArguments args, AppInstance keyInstance) {
        using EventWaitHandle redirectEventHandle = new(false, EventResetMode.ManualReset);

        Task.Run(() => {
            keyInstance.RedirectActivationToAsync(args).AsTask().Wait();
            redirectEventHandle.Set();
        });

        redirectEventHandle.WaitOne(int.MaxValue);
        _ = SetForegroundWindow(Process.GetProcessById((int)keyInstance.ProcessId).MainWindowHandle);

        [DllImport("user32.dll", ExactSpelling = true)]
        static extern bool SetForegroundWindow(IntPtr hWnd);
    }

    private static void OnActivated(object? sender, AppActivationArguments args) {
        var kind = args.Kind;
    }
}
