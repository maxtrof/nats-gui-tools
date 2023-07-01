using System;
using System.Reactive;
using Avalonia.Interactivity;

namespace UI.EventArgs;

public class DeleteServerRoutedEventArgs : RoutedEventArgs
{
    public Guid Id { get; set; }
}