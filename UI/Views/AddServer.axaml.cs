using System;
using Avalonia;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using UI.ViewModels;

namespace UI.Views;

public partial class AddServerWindow : ReactiveWindow<AddServerViewModel>
{
    public AddServerWindow()
    {
        InitializeComponent();
        this.WhenActivated(d => d(ViewModel!.AddServerCommand.Subscribe(Close)));
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

}