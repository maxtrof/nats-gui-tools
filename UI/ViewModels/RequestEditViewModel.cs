using Domain.Models;
using ReactiveUI;

namespace UI.ViewModels;

public class RequestEditViewModel : ViewModelBase
{
    private readonly RequestTemplate _requestTemplate;

    private string _name;
    private string _topic;
    private string _body;

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
    public string Topic {
        get => _topic;
        set=> this.RaiseAndSetIfChanged(ref _topic, value);
    }

    /// <summary>
    /// Request body
    /// </summary>
    public string Body {
        get => _body;
        set => this.RaiseAndSetIfChanged(ref _body, value);
    }
    
    public RequestEditViewModel()
    {
        _requestTemplate = new RequestTemplate()
        {
            Name = "New_request"
        };
        _name = _requestTemplate.Name;
        _topic = _requestTemplate.Topic;
        _body = _requestTemplate.Body;
    }
    
    public RequestEditViewModel(string name)
    {
        _requestTemplate = new RequestTemplate()
        {
            Name = name
        };
        _name = _requestTemplate.Name;
        _topic = _requestTemplate.Topic;
        _body = _requestTemplate.Body;
    }

    public RequestEditViewModel(RequestTemplate requestTemplate)
    {
        _requestTemplate = requestTemplate;
        _name = requestTemplate.Name;
        _topic = requestTemplate.Topic;
        _body = requestTemplate.Body;
    }
}