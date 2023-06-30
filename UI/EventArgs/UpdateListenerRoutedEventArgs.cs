using Avalonia.Interactivity;
using Domain.Models;

namespace UI.EventArgs;

internal class UpdateListenerRoutedEventArgs: RoutedEventArgs
{
    public Listener Listener { get; set; } = default!;
}