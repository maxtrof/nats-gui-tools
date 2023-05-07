using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace UI.Controls;

public partial class ServerListItemControl : UserControl
{
   public ServerListItemControl()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}