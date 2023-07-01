using System;
using System.Collections.ObjectModel;
using System.Reactive;
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

public sealed class MockEditViewModel : ViewModelBase, IActivatableViewModel, IDisposable
{
    private readonly MockEngine _mockEngine;
    private readonly IDataStorage _storage;

    private string _name = default!;
    private string _topic = default!;
    private string _answerTemplate = default!;

    public readonly Guid MockId = default!;
    private string? _validationError;
    private MockTypes _mockType;
    private bool _showReplySection;
    private int _mockRowSpan;
    private Guid? _activatedRule;

    /// <summary>
    /// Current activated rule
    /// </summary>
    private Guid? ActivatedRule
    {
        get => _activatedRule;
        set => this.RaiseAndSetIfChanged(ref _activatedRule, value);
    }

    public ObservableCollection<string> AutocompleteOptions => SharedObservables.Suggestions;

    /// <summary>
    /// Enable Mock
    /// </summary>
    public ICommand EnableMock { get; set; } = default!;
    /// <summary>
    /// Disable mock
    /// </summary>
    public ICommand DisableMock { get; set; } = default!;

    /// <inheritdoc />
    public ViewModelActivator Activator { get; } = default!;

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

    public MockTypes MockType
    {
        get => _mockType;
        set
        {
            this.RaiseAndSetIfChanged(ref _mockType, value);
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
        get => _mockRowSpan;
        set => this.RaiseAndSetIfChanged(ref _mockRowSpan, value);
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
        _mockEngine.OnRuleStopped += OnRuleStop;
        EnableMock = ReactiveCommand.CreateFromTask(async _ =>
        {
            ValidationError = ValidateForm();
            if (ValidationError != null)
                return;
            try
            {
                ActivatedRule = await _mockEngine.ActivateRule(MockTemplate);
            }
            catch (Exception ex)
            {
                ErrorHelper.ShowError(ex.Message);
            }
        });
        
        DisableMock =  ReactiveCommand.Create<Unit>(_ =>
        {
            ValidationError = ValidateForm();
            if (ValidationError != null)
                return;
            try
            {
                if (ActivatedRule == null)
                    return;
                _mockEngine.DeactivateRule(ActivatedRule.Value);
                ActivatedRule = null;
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
    
    private void OnRuleStop(object? sender, Guid id)
    {
        if (id == ActivatedRule)
        {
            ActivatedRule = null;
        }
    }

    public void Dispose()
    {
        _mockEngine.OnRuleStopped -= OnRuleStop;
        if (ActivatedRule == null) 
            return;
        _mockEngine.DeactivateRule(ActivatedRule.Value);
        _mockEngine.Dispose();
    }
}