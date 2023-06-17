using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using UI.EventArgs;
using UI.ViewModels;

namespace UI.Controls;

internal partial class RequestsTabControl : UserControl
{
    private readonly RequestsTabViewModel _vm;

    public static readonly RoutedEvent<UpdateRequestRoutedEventArgs> OnRequestUpdated =
        RoutedEvent.Register<RequestsTabControl, UpdateRequestRoutedEventArgs>(nameof(UpdateRequest),
            RoutingStrategies.Bubble);

    public event EventHandler<UpdateRequestRoutedEventArgs> UpdateRequest
    {
        add => AddHandler(OnRequestUpdated, value);
        remove => RemoveHandler(OnRequestUpdated, value);
    }

    public RequestsTabControl()
    {
        _vm = new RequestsTabViewModel();
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
        _vm.SelectedTab.RequestTemplate.Name = textBox.Text;
        RaiseEvent(new UpdateRequestRoutedEventArgs()
        {
            RequestTemplate = _vm.SelectedTab.RequestTemplate,
            RoutedEvent = OnRequestUpdated
        });
    }
}