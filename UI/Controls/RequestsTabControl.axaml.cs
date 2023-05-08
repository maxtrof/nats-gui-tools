using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using UI.ViewModels;

namespace UI.Controls;

public partial class RequestsTabControl : UserControl
{
    public RequestsTabControl()
    {
        var vm = new RequestsTabViewModel();
        DataContext = vm; 
        
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
        
}