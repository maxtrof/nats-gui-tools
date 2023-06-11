using System;
using System.Text;
using System.Windows.Input;
using Application.RequestProcessing;
using Autofac;
using Domain.Models;
using ReactiveUI;
using UI.MessagesBus;

namespace UI.ViewModels;

public class RequestEditViewModel : ViewModelBase
{
    private readonly RequestProcessor _requestProcessor;

    private string _name = default!;
    private string _topic = default!;
    private string _body = default!;
    private string _responseText = default!;

    public readonly Guid RequestId = default!;
    private string? _validationError;

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

    /// <summary>
    /// Current request template data
    /// </summary>
    public RequestTemplate RequestTemplate =>
        new()
        {
            Id = RequestId,
            Name = Name,
            Body = Body,
            Topic = Topic
        };

    public RequestEditViewModel()
    {
        RequestId = Guid.NewGuid();
        var scope = Program.Container.BeginLifetimeScope();
        _requestProcessor = scope.Resolve<RequestProcessor>();

        InitCommands();
    }

    public RequestEditViewModel(RequestTemplate requestTemplate)
    {
        var scope = Program.Container.BeginLifetimeScope();
        _requestProcessor = scope.Resolve<RequestProcessor>();
        RequestId = requestTemplate.Id;
        Name = requestTemplate.Name;
        Topic = requestTemplate.Topic;
        Body = requestTemplate.Body;

        InitCommands();
    }

    private void InitCommands()
    {
        ProcessRequest = ReactiveCommand.CreateFromTask(async _ =>
        {
            ValidationError = ValidateForm();
            if(ValidationError!= null)
                return;
            try
            {
                var result = await _requestProcessor.SendRequestReply(new RequestTemplate()
                {
                    Name = _name,
                    Body = _body,
                    Topic = _topic
                });
                ResponseText = result;
            }
            catch (Exception ex)
            {
                MessageBus.Current.SendMessage(ex.Message, BusEvents.ErrorThrown);
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