using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace EasyVhd.Models;

public static class Status {
    public static string? SelectedVhdFile {
        get;
        set {
            field = value;
            WeakReferenceMessenger.Default.Send(new ValueChangedMessage<string?>(value));
        }
    }
}
