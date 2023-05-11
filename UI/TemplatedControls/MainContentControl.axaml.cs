using Avalonia;
using Avalonia.Controls.Primitives;

namespace UI.TemplatedControls;

public class MainContentControl : TemplatedControl
{
    public static readonly StyledProperty<int> SelectedTabProperty = AvaloniaProperty.Register<MainContentControl, int>(
        nameof(SelectedTab));

    public int SelectedTab
    {
        get => GetValue(SelectedTabProperty);
        set => SetValue(SelectedTabProperty, value);
    }
}