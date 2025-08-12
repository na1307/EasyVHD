using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using EasyVhd.Models;

namespace EasyVhd.ViewModels;

public abstract partial class MainScreenViewModel : ViewModel {
    protected MainScreenViewModel() {
        WeakReferenceMessenger.Default.Register<ValueChangedMessage<string?>>(this, Handler);
        SelectedVhdText = Status.SelectedVhdFile is not null ? $"선택된 VHD 파일: {Status.SelectedVhdFile}" : "선택된 VHD 파일이 없습니다";
    }

    [ObservableProperty]
    public partial string SelectedVhdText { get; private set; }

    private void Handler(object recipient, ValueChangedMessage<string?> message)
        => SelectedVhdText = message.Value is not null ? $"선택된 VHD 파일: {message.Value}" : "선택된 VHD 파일이 없습니다";
}
