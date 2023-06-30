using Avalonia.Interactivity;
using Domain.Models;

namespace UI.EventArgs;

internal class UpdateRequestRoutedEventArgs: RoutedEventArgs
{
    public RequestTemplate RequestTemplate { get; set; } = default!;
}