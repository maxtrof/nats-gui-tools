using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using Domain.Models;
using ReactiveUI;
using UI.ViewModels;

namespace UI.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    public MainWindow()
    {
        InitializeComponent();
        this.WhenActivated(d => d(ViewModel!.ShowAddNewServerDialog.RegisterHandler(DoShowAddServerDialogAsync)));
        this.WhenActivated(d => d(ViewModel!.ShowSettingsWindowDialog.RegisterHandler(DoShowSettingsDialogAsync)));
        
    }
    
    private async Task DoShowAddServerDialogAsync(InteractionContext<AddServerViewModel, NatsServerSettings?> interaction)
    {
        var dialog = new AddServerWindow
        {
            DataContext = interaction.Input,
            Width = 400.0,
            MaxHeight = 500.0,
            Height = 500.0
        };

        var result = await dialog.ShowDialog<NatsServerSettings?>(this);
        interaction.SetOutput(result);
    }
    
    private async Task DoShowSettingsDialogAsync(InteractionContext<SettingsViewModel, Dictionary<string, string>?> interaction)
    {
        var dialog = new Settings
        {
            DataContext = interaction.Input,
            Width = 400.0,
            MaxHeight = 500.0,
            Height = 500.0
        };

        var result = await dialog.ShowDialog<Dictionary<string, string>?>(this);
        interaction.SetOutput(result);
    }

    private void ServerListItemControl_OnUpdateServersState(object? sender, RoutedEventArgs e)
    {
        ViewModel!.UpdateServersList();
    }
}