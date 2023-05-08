using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace UI.Controls;

public partial class RequestEditControl : UserControl
{
    public RequestEditControl()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}