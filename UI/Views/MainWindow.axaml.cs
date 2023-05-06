using System.Threading.Tasks;
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
        this.WhenActivated(d => d(ViewModel!.ShowAddNewServerDialog.RegisterHandler(DoShowDialogAsync)));
        
    }
    
    private async Task DoShowDialogAsync(InteractionContext<AddServerViewModel, NatsServerSettings?> interaction)
    {
        var dialog = new AddServerWindow
        {
            DataContext = interaction.Input
        };

        var result = await dialog.ShowDialog<NatsServerSettings?>(this);
        interaction.SetOutput(result);
    }
}