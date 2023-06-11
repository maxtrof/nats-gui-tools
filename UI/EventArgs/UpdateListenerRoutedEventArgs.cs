using Avalonia.Interactivity;
using Domain.Models;

namespace UI.EventArgs;

public class UpdateListenerRoutedEventArgs: RoutedEventArgs
{
    public Listener Listener { get; set; }
}