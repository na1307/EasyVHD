using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using EasyVhd.Views;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.Storage.Pickers;

namespace EasyVhd.ViewModels;

public abstract partial class SubScreenViewModel(Screen previousScreen) : ViewModel {
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ProgressActive))]
    public partial bool ControlsEnabled { get; private set; } = true;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [VhdPath]
    public partial string VhdPath { get; set; } = string.Empty;

    public bool ProgressActive => !ControlsEnabled;

    protected async Task<bool> Run(Screen screen, Func<Task> command) {
        disableControls();

        ContentDialog cd = new() {
            XamlRoot = screen.XamlRoot,
            Style = (Style)Application.Current.Resources["DefaultContentDialogStyle"],
            Title = "오류",
            PrimaryButtonText = "확인",
            DefaultButton = ContentDialogButton.Primary
        };

        ValidateAllProperties();

        if (HasErrors) {
            cd.Content = GetErrors().First().ErrorMessage;

            await cd.ShowAsync();
            enableControls();

            return false;
        }

        try {
            await command();
        } catch (Exception e) {
            cd.Content = e.Message;

            await cd.ShowAsync();
            enableControls();

            return false;
        }

        enableControls();

        return true;
    }

    [RelayCommand]
    protected void Previous() => Ioc.Default.GetRequiredService<MainWindowViewModel>().CurrentScreen = previousScreen;

    [RelayCommand]
    private async Task OpenVhdPathBrowse() {
        FileOpenPicker picker = new(App.Current.Window!.AppWindow.Id);

        picker.FileTypeFilter.Add(".vhd");
        picker.FileTypeFilter.Add(".vhdx");

        var file = await picker.PickSingleFileAsync();

        if (file is not null) {
            VhdPath = file.Path;
        }
    }

    private void disableControls() => ControlsEnabled = false;

    private void enableControls() => ControlsEnabled = true;
}
