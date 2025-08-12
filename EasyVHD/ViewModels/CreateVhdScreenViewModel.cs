using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasyVhd.Models;
using EasyVhd.Views;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.Storage.Pickers;
using System.ComponentModel.DataAnnotations;

namespace EasyVhd.ViewModels;

public sealed partial class CreateVhdScreenViewModel(Screen previousScreen) : SubScreenViewModel(previousScreen) {
    private AfterCreateAction after;
    private CancellationTokenSource? cts;
    private bool notCanceling = true;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SizeEnabled))]
    [NotifyPropertyChangedFor(nameof(ProgressActive))]
    [NotifyPropertyChangedFor(nameof(CreateOrCancel))]
    public new partial bool ControlsEnabled { get; private set; } = true;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [VhdPath(false, true)]
    public new partial string VhdPath { get; set; } = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsDifferencing))]
    [NotifyPropertyChangedFor(nameof(SourceText))]
    [NotifyPropertyChangedFor(nameof(SizeEnabled))]
    public partial VhdType VhdType { get; set; }

    public bool IsDifferencing => VhdType == VhdType.Differencing;

    public string SourceText => IsDifferencing ? "원본(부모) VHD 경로" : "원본 VHD 경로(지정하지 않으면 빈 VHD가 만들어짐)";

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [CustomValidation(typeof(CreateVhdScreenViewModel), nameof(ValidateParent))]
    [VhdPath(true, false)]
    public partial string SourcePath { get; set; } = string.Empty;

    public bool SizeEnabled => ControlsEnabled && !IsDifferencing;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [CustomValidation(typeof(CreateVhdScreenViewModel), nameof(ValidateSize))]
    public partial string VhdSize { get; set; } = 0.ToString();

    [ObservableProperty]
    public partial SizeUnit SizeUnit { get; set; }

    public new bool ProgressActive => !ControlsEnabled;

    public string CreateOrCancel => ControlsEnabled ? "만들기" : "취소";

    public SizeUnit[] SizeUnits => Enum.GetValues<SizeUnit>();

    public static ValidationResult? ValidateSize(string sSize, ValidationContext context) {
        var vm = (CreateVhdScreenViewModel)context.ObjectInstance;

        if (string.IsNullOrWhiteSpace(sSize)) {
            return new("VHD 크기는 비워둘 수 없습니다.");
        }

        if (!ulong.TryParse(sSize, out var rawSize)) {
            return new("VHD 크기는 숫자여야 합니다.");
        }

        var size = ToByte(rawSize, vm.SizeUnit);
        var format = Path.GetExtension(vm.VhdPath).TrimStart('.');

        if ((format.Equals("vhd", StringComparison.OrdinalIgnoreCase) && size is > 2199023255552UL or < 3145728UL) || (format.Equals("vhdx", StringComparison.OrdinalIgnoreCase) && size is > 70368744177664UL or < 3145728UL)) {
            return new("크기가 너무 크거나 작습니다.");
        }

        return ValidationResult.Success;
    }

    public static ValidationResult? ValidateParent(string path, ValidationContext context)
        => ((CreateVhdScreenViewModel)context.ObjectInstance).IsDifferencing && string.IsNullOrWhiteSpace(path) ? new("VHD 경로는 비어 있지 않아야 합니다.") : ValidationResult.Success;

    private static ulong ToByte(ulong rawSize, SizeUnit sizeUnit) => sizeUnit switch {
        SizeUnit.MB => rawSize * 1024 * 1024,
        SizeUnit.GB => rawSize * 1024 * 1024 * 1024,
        SizeUnit.TB => rawSize * 1024 * 1024 * 1024 * 1024,
        _ => throw new InvalidOperationException()
    };

    [RelayCommand]
    private async Task VhdPathBrowse() {
        FileSavePicker picker = new(App.Current.Window!.AppWindow.Id) { SuggestedFileName = VhdPath };

        picker.FileTypeChoices.Add("VHD 파일", [".vhd"]);
        picker.FileTypeChoices.Add("VHDX 파일", [".vhdx"]);

        var file = await picker.PickSaveFileAsync();

        if (file is not null) {
            File.Delete(file.Path);
            VhdPath = file.Path;
        }
    }

    [RelayCommand]
    private void Expandable() => VhdType = VhdType.Expandable;

    [RelayCommand]
    private void Fixed() => VhdType = VhdType.Fixed;

    [RelayCommand]
    private void Differencing() => VhdType = VhdType.Differencing;

    [RelayCommand]
    private async Task SourcePathBrowse() {
        FileOpenPicker picker = new(App.Current.Window!.AppWindow.Id);

        picker.FileTypeFilter.Add(".vhd");
        picker.FileTypeFilter.Add(".vhdx");

        var file = await picker.PickSingleFileAsync();

        if (file is not null) {
            SourcePath = file.Path;
        }
    }

    [RelayCommand]
    private void Select() => after = AfterCreateAction.Select;

    [RelayCommand]
    private void Attach() => after = AfterCreateAction.Attach;

    [RelayCommand]
    private void Nothing() => after = AfterCreateAction.Nothing;

    [RelayCommand]
    private async Task CreateVhdFile(CreateVhdScreen screen) {
        if (ControlsEnabled) {
            notCanceling = true;
            _ = createVhdFileCore(screen);
        } else if (notCanceling) {
            notCanceling = false;
            await cts!.CancelAsync();
            cts.Dispose();
            cts = null;
        }
    }

    private async Task createVhdFileCore(CreateVhdScreen screen) {
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

            return;
        }

        try {
            using (cts = new()) {
                using var handle = await VhdFunctions.CreateVirtualDiskAsync(VhdPath, ToByte(ulong.Parse(VhdSize), SizeUnit), VhdType, SourcePath, cts.Token);

                if (after == AfterCreateAction.Attach) {
                    await VhdFunctions.AttachVirtualDiskAsync(handle, false);
                }
            }
        } catch (Exception e) {
            cd.Content = e.Message;

            await cd.ShowAsync();
            enableControls();

            return;
        }

        enableControls();

        if (after is AfterCreateAction.Select or AfterCreateAction.Attach) {
            Status.SelectedVhdFile = VhdPath;
            Previous();
        }
    }

    private void disableControls() => ControlsEnabled = false;

    private void enableControls() => ControlsEnabled = true;
}
