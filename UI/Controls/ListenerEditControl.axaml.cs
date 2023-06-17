using System;
using Application.RequestProcessing;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace UI.Controls;

internal partial class ListenerEditControl : UserControl
{
    public static readonly RoutedEvent<RoutedEventArgs> UpdateRequestDataEvent =
        RoutedEvent.Register<ListenerEditControl, RoutedEventArgs>(nameof(UpdateRequestData), RoutingStrategies.Bubble);
    
    public event EventHandler<RoutedEventArgs> UpdateRequestData
    { 
        add => AddHandler(UpdateRequestDataEvent, value);
        remove => RemoveHandler(UpdateRequestDataEvent, value);
    }
    public ListenerEditControl()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void MessageTextBlock_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is TextBlock block)
            Avalonia.Application.Current?.Clipboard?.SetTextAsync(block.Text);
    }
}