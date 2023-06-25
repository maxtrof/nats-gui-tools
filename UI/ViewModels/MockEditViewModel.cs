using System;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Text;
using System.Windows.Input;
using Application.MockEngine;
using Autofac;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Models;
using ReactiveUI;
using UI.Helpers;
using UI.MessagesBus;

namespace UI.ViewModels;

internal sealed class MockEditViewModel : ViewModelBase, IActivatableViewModel
{
    private readonly MockEngine _mockEngine;
    private readonly IDataStorage _storage;

    private string _name = default!;
    private string _topic = default!;
    private string _answerTemplate = default!;
    private string _responseText = default!;

    public readonly Guid MockId = default!;
    private string? _validationError;
    private MockTypes _MockType;
    private bool _showReplySection;
    private int _MockRowSpan;

    /// <summary>
    /// Current activated rule
    /// </summary>
    private Guid? ActivatedRule { get; set; }

    public ObservableCollection<string> AutocompleteOptions => SharedObservables.Suggestions;

    /// <summary> Process Mock </summary>
    public ICommand ProcessMock { get; set; } = default!;

    /// <inheritdoc />
    public ViewModelActivator Activator { get; }

    /// <summary>
    /// Template name
    /// </summary>
    public string Name
    {
        get => _name;
        set
        {
            this.RaiseAndSetIfChanged(ref _name, value);
            BroadcastMockTemplateUpdated();
        }
    }

    /// <summary>
    /// Topic to send Mock
    /// </summary>
    public string Topic
    {
        get => _topic;
        set
        {
            this.RaiseAndSetIfChanged(ref _topic, value);
            BroadcastMockTemplateUpdated();
        }
    }

    public string? ValidationError
    {
        get => _validationError;
        set => this.RaiseAndSetIfChanged(ref _validationError, value);
    }

    /// <summary>
    /// Mock body
    /// </summary>
    public string AnswerTemplate
    {
        get => _answerTemplate;
        set
        {
            this.RaiseAndSetIfChanged(ref _answerTemplate, value);
            BroadcastMockTemplateUpdated();
        }
    }

    public string ResponseText
    {
        get => _responseText;
        set => this.RaiseAndSetIfChanged(ref _responseText, value);
    }

    public MockTypes MockType
    {
        get => _MockType;
        set
        {
            this.RaiseAndSetIfChanged(ref _MockType, value);
            BroadcastMockTemplateUpdated();
        }
    }

    public bool ShowReplySection
    {
        get => _showReplySection;
        set
        {
            this.RaiseAndSetIfChanged(ref _showReplySection, value);
            MockRowSpan = value
                ? 1
                : 2;
        }
    }

    public int MockRowSpan
    {
        get => _MockRowSpan;
        set => this.RaiseAndSetIfChanged(ref _MockRowSpan, value);
    }

    /// <summary>
    /// Current Mock template data
    /// </summary>
    public MockTemplate MockTemplate =>
        new()
        {
            Id = MockId,
            Name = Name,
            AnswerTemplate = AnswerTemplate,
            Topic = Topic,
            Type = MockType
        };

    public MockEditViewModel()
    {
        MockId = Guid.NewGuid();
        var scope = Program.Container.BeginLifetimeScope();
        _mockEngine = scope.Resolve<MockEngine>();
        _storage = scope.Resolve<IDataStorage>();
        MockType = MockTypes.SimpleRequestReply;

        Init();
    }

    public MockEditViewModel(MockTemplate mockTemplate)
    {
        Activator = new ViewModelActivator();
        this.WhenActivated(disposables =>
        {
            Disposable
                .Create(Dispose)
                .DisposeWith(disposables);
        });

        var scope = Program.Container.BeginLifetimeScope();
        _mockEngine = scope.Resolve<MockEngine>();
        _storage = scope.Resolve<IDataStorage>();
        MockId = mockTemplate.Id;
        Name = mockTemplate.Name;
        Topic = mockTemplate.Topic;
        AnswerTemplate = mockTemplate.AnswerTemplate;
        MockType = mockTemplate.Type;

        Init();
    }

    private void Init()
    {
        ProcessMock = ReactiveCommand.CreateFromTask(async _ =>
        {
            ValidationError = ValidateForm();
            if (ValidationError != null)
                return;
            try
            {
                switch (MockType)
                {
                    case MockTypes.SimpleRequestReply:
                        ActivatedRule = await _mockEngine.ActivateRule(MockTemplate);
                        ResponseText = "";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("Unknown Mock type");
                }
            }
            catch (Exception ex)
            {
                ErrorHelper.ShowError(ex.Message);
            }
        });
    }

    private void BroadcastMockTemplateUpdated() => 
        MessageBus.Current.SendMessage(MockTemplate, BusEvents.MockUpdated);

    private string? ValidateForm()
    {
        var sb = new StringBuilder();
        if (string.IsNullOrWhiteSpace(Topic)) sb.AppendLine("Topic is empty");
        if (string.IsNullOrWhiteSpace(AnswerTemplate)) sb.AppendLine("Mock answer template is empty");
        return sb.Length > 0 ? sb.ToString() : null;
    }

    private void Dispose()
    {
        if (ActivatedRule == null) return;
        _mockEngine.DeactivateRule(ActivatedRule.Value);
        _mockEngine.Dispose();
    }
}