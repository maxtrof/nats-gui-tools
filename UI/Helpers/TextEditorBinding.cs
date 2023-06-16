using System;
using Avalonia;
using Avalonia.Xaml.Interactivity;
using AvaloniaEdit;
using AvaloniaEdit.Document;

namespace UI.Helpers;

/// <summary>
/// Behavior for reactive binding for <see cref="TextEditor"/>
/// </summary>
public class DocumentTextBindingBehavior : Behavior<TextEditor>
{
    private TextEditor? _textEditor = null;

    public static readonly StyledProperty<string> TextProperty =
        AvaloniaProperty.Register<DocumentTextBindingBehavior, string>(nameof(Text));

    public string Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    protected override void OnAttached()
    {
        base.OnAttached();

        if (AssociatedObject is TextEditor textEditor)
        {
            _textEditor = textEditor;
            _textEditor.TextChanged += TextChanged;
            this.GetObservable(TextProperty).Subscribe(TextPropertyChanged);
        }
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();

        if (_textEditor != null)
        {
            _textEditor.TextChanged -= TextChanged;
        }
    }

    private void TextChanged(object? sender, System.EventArgs eventArgs)
    {
        if (_textEditor is { Document: not null })
        {
            Text = _textEditor.Document.Text;
        }
    }

    private void TextPropertyChanged(string text)
    {
        if (_textEditor is { Document: not null })
        {
            if (string.IsNullOrWhiteSpace(text))
                text = "";
            var caretOffset = _textEditor.CaretOffset;
            _textEditor.Document.Text = text;
            _textEditor.CaretOffset = caretOffset;
        }
    }
}