using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using Application.RequestProcessing;
using Autofac;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Models;
using ReactiveUI;
using UI.Helpers;
using UI.MessagesBus;

namespace UI.ViewModels;

internal sealed class RequestEditViewModel : ViewModelBase
{
    private readonly RequestProcessor _requestProcessor;
    private readonly IDataStorage _storage;

    private string _name = default!;
    private string _topic = default!;
    private string _body = default!;
    private string _responseText = default!;

    public readonly Guid RequestId = default!;
    private string? _validationError;
    private RequestType _requestType;
    private bool _showReplySection;
    private int _requestRowSpan;
    
    public ObservableCollection<string> AutocompleteOptions => SharedObservables.Suggestions;
    /// <summary> Process request </summary>
    public ICommand ProcessRequest { get; set; } = default!;

    /// <summary>
    /// Template name
    /// </summary>
    public string Name
    {
        get => _name;
        set
        {
            this.RaiseAndSetIfChanged(ref _name, value);
            BroadcastRequestTemplateUpdated();
        }
    }

    /// <summary>
    /// Topic to send Request
    /// </summary>
    public string Topic
    {
        get => _topic;
        set
        {
            this.RaiseAndSetIfChanged(ref _topic, value);
            BroadcastRequestTemplateUpdated();
        }
    }

    public string? ValidationError
    {
        get => _validationError;
        set => this.RaiseAndSetIfChanged(ref _validationError, value);
    }

    /// <summary>
    /// Request body
    /// </summary>
    public string Body
    {
        get => _body;
        set
        {
            this.RaiseAndSetIfChanged(ref _body, value);
            BroadcastRequestTemplateUpdated();
        }
    }

    public string ResponseText
    {
        get => _responseText;
        set => this.RaiseAndSetIfChanged(ref _responseText, value);
    }

    public RequestType RequestType
    {
        get => _requestType;
        set
        {
            this.RaiseAndSetIfChanged(ref _requestType, value);
            ShowReplySection = value == RequestType.RequestReply;
            BroadcastRequestTemplateUpdated();
        }
    }

    public bool ShowReplySection
    {
        get => _showReplySection;
        set
        {
            this.RaiseAndSetIfChanged(ref _showReplySection, value);
            RequestRowSpan = value
                    ? 1
                    : 2;
        }
    }

    public int RequestRowSpan
    {
        get => _requestRowSpan;
        set => this.RaiseAndSetIfChanged(ref _requestRowSpan, value);
    }

    /// <summary>
    /// Current request template data
    /// </summary>
    public RequestTemplate RequestTemplate =>
        new()
        {
            Id = RequestId,
            Name = Name,
            Body = Body,
            Topic = Topic,
            Type = RequestType
        };

    public RequestEditViewModel()
    {
        RequestId = Guid.NewGuid();
        var scope = Program.Container.BeginLifetimeScope();
        _requestProcessor = scope.Resolve<RequestProcessor>();
        _storage = scope.Resolve<IDataStorage>();
        RequestType = RequestType.Publish;

        Init();
    }

    public RequestEditViewModel(RequestTemplate requestTemplate)
    {
        var scope = Program.Container.BeginLifetimeScope();
        _requestProcessor = scope.Resolve<RequestProcessor>();
        _storage = scope.Resolve<IDataStorage>();
        RequestId = requestTemplate.Id;
        Name = requestTemplate.Name;
        Topic = requestTemplate.Topic;
        Body = requestTemplate.Body;
        RequestType = requestTemplate.Type;

        Init();
    }

    private void Init()
    {
        ProcessRequest = ReactiveCommand.CreateFromTask(async _ =>
        {
            ValidationError = ValidateForm();
            if(ValidationError!= null)
                return;
            try
            {
                switch (RequestType)
                {
                    case RequestType.Publish:
                        await _requestProcessor.SendRequest(RequestTemplate);
                        ResponseText = "";
                        break;
                    case RequestType.RequestReply:
                        var result = await _requestProcessor.SendRequestReply(RequestTemplate);
                        ResponseText = _storage.AppSettings.FormatJson
                            ? JsonFormatter.TryFormatJson(result)
                            : result;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("Unknown request type");
                }
            }
            catch (Exception ex)
            {
                ErrorHelper.ShowError(ex.Message);
            }
        });
    }

    private void BroadcastRequestTemplateUpdated()
    {
        MessageBus.Current.SendMessage(RequestTemplate, BusEvents.RequestUpdated);
    }

    private string? ValidateForm()
    {
        var sb = new StringBuilder();
        if (string.IsNullOrWhiteSpace(Topic)) sb.AppendLine("Topic is empty");
        if (string.IsNullOrWhiteSpace(Body)) sb.AppendLine("Request body is empty");
        return sb.Length > 0 ? sb.ToString() : null;
    }
}