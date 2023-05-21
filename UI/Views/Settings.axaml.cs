using System;
using System.ComponentModel;
using Avalonia;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using DynamicData.Binding;
using ReactiveUI;
using UI.ViewModels;

namespace UI.Views;

public partial class Settings : ReactiveWindow<SettingsViewModel>
{
    public Settings()
    {
        InitializeComponent();
        this.WhenActivated(d => d(ViewModel!.UpdateUserDictionaryCommand.Subscribe(Close)));
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}