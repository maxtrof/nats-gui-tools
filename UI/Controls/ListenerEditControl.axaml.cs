using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ReactiveUI;
using UI.MessagesBus;

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

    private void TopicAutoCompleteBox_OnLostFocus(object? sender, RoutedEventArgs e)
    {
        if (sender is AutoCompleteBox autoCompleteBox)
            MessageBus.Current.SendMessage(autoCompleteBox.Text, BusEvents.AutocompleteAdded);
    }
}