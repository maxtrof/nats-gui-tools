using ReactiveUI;
using UI.MessagesBus;

namespace UI.Helpers;

public static class ErrorHelper
{
    public static void ShowError(string message)
    {
        MessageBus.Current.SendMessage(message, BusEvents.ErrorThrown);
    }
}