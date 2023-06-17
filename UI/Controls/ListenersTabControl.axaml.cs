using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using UI.EventArgs;
using UI.ViewModels;

namespace UI.Controls;

internal partial class ListenersTabControl : UserControl
{
    private readonly ListenersTabViewModel _vm;

    public static readonly RoutedEvent<UpdateListenerRoutedEventArgs> OnRequestUpdated =
        RoutedEvent.Register<ListenersTabControl, UpdateListenerRoutedEventArgs>(nameof(UpdateRequest),
            RoutingStrategies.Bubble);

    public event EventHandler<UpdateListenerRoutedEventArgs> UpdateRequest
    {
        add => AddHandler(OnRequestUpdated, value);
        remove => RemoveHandler(OnRequestUpdated, value);
    }

    public ListenersTabControl()
    {
        _vm = new ListenersTabViewModel();
        DataContext = _vm;

        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void EditTitleField_OnGotFocus(object? sender, GotFocusEventArgs e)
    {
        if (sender is not TextBox textBox)
            return;
        var name = textBox.Text;
        _vm.SetSelectedTab(name);
    }

    private void EditTitleField_OnLostFocus(object? sender, RoutedEventArgs _)
    {
        if (sender is not TextBox textBox || _vm.SelectedTab is null)
            return;
        _vm.SelectedTab.Listener.Name = textBox.Text;
        RaiseEvent(new UpdateListenerRoutedEventArgs()
        {
            Listener = _vm.SelectedTab.Listener,
            RoutedEvent = OnRequestUpdated
        });
    }
}