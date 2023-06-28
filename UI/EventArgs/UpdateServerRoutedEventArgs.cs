using System;
using System.Reactive;
using Avalonia.Interactivity;

namespace UI.EventArgs;

public class UpdateServerRoutedEventArgs : RoutedEventArgs
{
    public Guid Id { get; set; }
}