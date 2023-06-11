using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using Domain.Models;
using DynamicData;
using ReactiveUI;
using UI.EventArgs;
using UI.ViewModels;

namespace UI.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    private static readonly List<FileDialogFilter> FileFilters = new()
    {
        new FileDialogFilter
        {
            Name = "Nats Gui Tools export files",
            Extensions = new List<string> {"ngt"}
        }
    };

    public MainWindow()
    {
        InitializeComponent();
        this.WhenActivated(d => d(ViewModel!.ShowAddNewServerDialog.RegisterHandler(DoShowAddServerDialogAsync)));
        this.WhenActivated(d => d(ViewModel!.YesNoDialog.RegisterHandler(DoShowYesNoDialogDialogAsync)));

        this.WhenActivated(d => d(ViewModel!.ShowSettingsWindowDialog.RegisterHandler(DoShowSettingsDialogAsync)));
        this.WhenActivated(d => d(ViewModel!.ShowExportFileSaveDialog.RegisterHandler(DoShowExportDialogAsync)));
        this.WhenActivated(d => d(ViewModel!.ShowImportFileLoadDialog.RegisterHandler(DoShowImportDialogAsync)));
    }

    private async Task DoShowAddServerDialogAsync(
        InteractionContext<AddServerViewModel, NatsServerSettings?> interaction)
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

    private async Task DoShowSettingsDialogAsync(
        InteractionContext<SettingsViewModel, Dictionary<string, string>?> interaction)
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

    private async Task DoShowExportDialogAsync(InteractionContext<Unit, string?> interaction)
    {
        var dialog = new SaveFileDialog
        {
            Filters = FileFilters
        };
        var result = await dialog.ShowAsync(this);
        interaction.SetOutput(result);
    }

    private async Task DoShowImportDialogAsync(InteractionContext<Unit, string?> interaction)
    {
        var dialog = new OpenFileDialog
        {
            Filters = FileFilters,
            AllowMultiple = false
        };
        var result = await dialog.ShowAsync(this);
        interaction.SetOutput(result?.FirstOrDefault());
    }

    private async Task DoShowYesNoDialogDialogAsync(InteractionContext<YesNoDialogViewModel, DialogResult> interaction)
    {
        var dialog = new YesNoDialog
        {
            DataContext = interaction.Input,
            Width = 600.0,
            MaxHeight = 200.0
        };
        var result = await dialog.ShowDialog<DialogResult>(this);
        interaction.SetOutput(result ?? new DialogResult()
        {
            Result = DialogResultEnum.None
        });
    }

    private void ServerListItemControl_OnUpdateServersState(object? sender, RoutedEventArgs e)
    {
        ViewModel!.UpdateServersList();
    }

    private void RequestsTabControl_OnUpdateRequest(object? sender, UpdateRequestRoutedEventArgs e)
    {
        var request = e.RequestTemplate;
        var exists = ViewModel!.RequestTemplates.FirstOrDefault(x => x.Id == request.Id);
        if (exists is not null)
        {
            ViewModel.RequestTemplates.Replace(exists, request);
        }
    }

    private void ListenersTabControl_OnUpdateRequest(object? sender, UpdateListenerRoutedEventArgs e)
    {
        var request = e.Listener;
        var exists = ViewModel!.Listeners.FirstOrDefault(x => x.Id == request.Id);
        if (exists is not null)
        {
            ViewModel.Listeners.Replace(exists, request);
        }
    }
}