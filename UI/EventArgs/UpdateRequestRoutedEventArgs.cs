using Avalonia.Interactivity;
using Domain.Models;

namespace UI.EventArgs;

public class UpdateRequestRoutedEventArgs: RoutedEventArgs
{
    public RequestTemplate RequestTemplate { get; set; }
}