using System;
using Avalonia;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using UI.ViewModels;

namespace UI.Views;

internal partial class YesNoDialog : ReactiveWindow<YesNoDialogViewModel>
{
    public YesNoDialog()
    {
        InitializeComponent();
        this.WhenActivated(d => d(ViewModel!.SetResult.Subscribe(Close)));
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}