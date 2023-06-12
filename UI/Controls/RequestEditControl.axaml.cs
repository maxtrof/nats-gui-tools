using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Domain.Enums;

namespace UI.Controls;

public partial class RequestEditControl : UserControl
{
    public static readonly RoutedEvent<RoutedEventArgs> UpdateRequestDataEvent =
        RoutedEvent.Register<RequestEditControl, RoutedEventArgs>(nameof(UpdateRequestData), RoutingStrategies.Bubble);
    
    public event EventHandler<RoutedEventArgs> UpdateRequestData
    { 
        add => AddHandler(UpdateRequestDataEvent, value);
        remove => RemoveHandler(UpdateRequestDataEvent, value);
    }
    public RequestEditControl()
    {
        InitializeComponent();
        var requestTypeComboBox = this.FindControl<ComboBox>("RequestTypeComboBox");
        requestTypeComboBox.Items = Enum.GetNames(typeof(RequestType));
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}