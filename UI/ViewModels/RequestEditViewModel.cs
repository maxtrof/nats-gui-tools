using System;
using System.Reactive;
using System.Windows.Input;
using Application.RequestProcessing;
using Autofac;
using Autofac.Core;
using Domain.Models;
using ReactiveUI;

namespace UI.ViewModels;

public class RequestEditViewModel : ViewModelBase
{
    private readonly RequestProcessor _requestProcessor;
    private readonly RequestTemplate _requestTemplate;

    private string _name;
    private string _topic;
    private string _body;
    private readonly ILifetimeScope _scope;
    private string _responseText;

    /// <summary> Process request </summary>
    public ICommand ProcessRequest { get; set; }

    /// <summary>
    /// Template name
    /// </summary>
    public string Name
    {
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }

    /// <summary>
    /// Topic to send Request
    /// </summary>
    public string Topic
    {
        get => _topic;
        set => this.RaiseAndSetIfChanged(ref _topic, value);
    }

    /// <summary>
    /// Request body
    /// </summary>
    public string Body
    {
        get => _body;
        set => this.RaiseAndSetIfChanged(ref _body, value);
    }

    public string ResponseText
    {
        get => _responseText;
        set => this.RaiseAndSetIfChanged(ref _responseText, value);
    }

    public RequestEditViewModel()
    {
        _scope = Program.Container.BeginLifetimeScope();
        _requestProcessor = _scope.Resolve<RequestProcessor>();
       
        InitCommands();
    }

    public RequestEditViewModel(string name)
    {
        _scope = Program.Container.BeginLifetimeScope();
        _requestProcessor = _scope.Resolve<RequestProcessor>();

        _requestTemplate = new RequestTemplate
        {
            Name = name
        };
        _name = _requestTemplate.Name;
        _topic = _requestTemplate.Topic;
        _body = _requestTemplate.Body;
        
        InitCommands();
    }

    private void InitCommands()
    {
        ProcessRequest = ReactiveCommand.CreateFromTask(async _ =>
        {
            var result = await _requestProcessor.SendRequestReply(new RequestTemplate()
            {
                Name = _name,
                Body = _body,
                Topic = _topic
            });
            this.ResponseText = result;
        });
    }

    public RequestEditViewModel(RequestTemplate requestTemplate)
    {
        _scope = Program.Container.BeginLifetimeScope();
        _requestProcessor = _scope.Resolve<RequestProcessor>();

        _requestTemplate = requestTemplate;
        _name = requestTemplate.Name;
        _topic = requestTemplate.Topic;
        _body = requestTemplate.Body;
        
        InitCommands();
    }
}