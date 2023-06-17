using ReactiveUI;
using UI.MessagesBus;

namespace UI.Helpers;

internal static class ErrorHelper
{
    /// <summary>
    /// Dispatch error message to the MessageBus.
    /// </summary>
    /// <param name="message"></param>
    public static void ShowError(string message)
    {
        MessageBus.Current.SendMessage(message, BusEvents.ErrorThrown);
    }
}