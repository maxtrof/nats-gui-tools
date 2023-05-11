using System.Reactive;
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
        this.WhenActivated(d => d(ViewModel!.YesNoDialog.RegisterHandler(DoShowYesNoDialogDialogAsync)));
        
    }
    
    private async Task DoShowAddServerDialogAsync(InteractionContext<AddServerViewModel, NatsServerSettings?> interaction)
    {
        var dialog = new AddServerWindow
        {
            DataContext = interaction.Input,
            Width = 400.0,
            MaxHeight = 500.0
        };

        var result = await dialog.ShowDialog<NatsServerSettings?>(this);
        interaction.SetOutput(result);
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
}