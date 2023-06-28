using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using UI.EventArgs;
using UI.ViewModels;

namespace UI.Controls;

internal partial class MocksTabControl : UserControl
{
    private readonly MocksTabViewModel _vm;

    public MocksTabControl()
    {
        _vm = new MocksTabViewModel();
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
        _vm.SelectedTab.Name = textBox.Text;
        // RaiseEvent(new UpdateMockRoutedEventArgs()
        // {
        //     Mock = _vm.SelectedTab.Mock,
        //     RoutedEvent = OnRequestUpdated
        // });
    }
}