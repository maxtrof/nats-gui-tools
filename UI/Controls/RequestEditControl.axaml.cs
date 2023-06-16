using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using AvaloniaEdit;
using AvaloniaEdit.Highlighting;
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
        // Editors json highlighting
        var requestEditor = this.FindControl<TextEditor>("RequestEditor");
        var replyEditor = this.FindControl<TextEditor>("ReplyEditor");
        var hlDefinition = HighlightingManager.Instance.GetDefinitionByExtension(".jsondark");
        
        replyEditor.SyntaxHighlighting = hlDefinition;
        requestEditor.SyntaxHighlighting = hlDefinition;
        
        replyEditor.Options.EnableHyperlinks = false;
        requestEditor.Options.EnableHyperlinks = false;
        
        requestEditor.Options.HighlightCurrentLine = true;
        replyEditor.Options.HighlightCurrentLine = true;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

}