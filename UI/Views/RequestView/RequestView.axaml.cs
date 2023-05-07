using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace UI.Views.RequestView;

public partial class RequestView : UserControl
{
    public RequestView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}