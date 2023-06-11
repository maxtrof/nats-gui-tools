using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Windows.Input;
using Application.RequestProcessing;
using Application.TopicListener;
using Autofac;
using Avalonia.Threading;
using Domain.Models;
using ReactiveUI;
using UI.MessagesBus;

namespace UI.ViewModels;

public class ListenerEditViewModel : ViewModelBase, IDisposable
{
    private readonly TopicListener _topicListener;

    private long? _topicId = null; 

    private string _name = default!;
    private string _topic = default!;
    private string? _validationError;

    public ObservableCollection<IncomingMessageData> Messages { get; set; }
    public readonly Guid ListenerId = default!;
    private bool _listening;

    /// <summary> Starts listening </summary>
    public ICommand StartListen { get; set; } = default!;
    /// <summary> Stops listening </summary>
    public ICommand StopListen { get; set; } = default!;

    /// <summary>
    /// Template name
    /// </summary>
    public string Name
    {
        get => _name;
        set
        {
            this.RaiseAndSetIfChanged(ref _name, value);
            BroadcastListenerUpdated();
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
            BroadcastListenerUpdated();
        }
    }

    /// <summary>
    /// Is currently listening
    /// </summary>
    public bool Listening
    {
        get => _listening;
        set
        {
            this.RaiseAndSetIfChanged(ref _listening, value);
            BroadcastListenerUpdated();
        }
    }

    public string? ValidationError
    {
        get => _validationError;
        set => this.RaiseAndSetIfChanged(ref _validationError, value);
    }

    /// <summary>
    /// Current request template data
    /// </summary>
    public Listener Listener =>
        new()
        {
            Id = ListenerId,
            Name = Name,
            Topic = Topic
        };

    public ListenerEditViewModel()
    {
        ListenerId = Guid.NewGuid();
        var scope = Program.Container.BeginLifetimeScope();
        _topicListener = scope.Resolve<TopicListener>();
        Messages = new();

        InitCommands();
    }

    public ListenerEditViewModel(Listener listener)
    {
        var scope = Program.Container.BeginLifetimeScope();
        _topicListener = scope.Resolve<TopicListener>();
        ListenerId = listener.Id;
        Name = listener.Name;
        Topic = listener.Topic;
        
        Messages = new (_topicListener.GetMessages(listener.Topic) ?? new List<IncomingMessageData>());

        InitCommands();
    }

    private void InitCommands()
    {
        // TODO: Catch errors and show messages to user (UI)
        StartListen = ReactiveCommand.Create<Unit>(_ =>
        {
            ValidationError = ValidateForm();
            if(ValidationError is not null)
                return;
            _topicListener.OnMessageReceived += MessageReceived;
            _topicId = _topicListener.SubscribeToListen(Topic);
            Listening = true;
        });
        StopListen = ReactiveCommand.Create<Unit>(_ =>
        {
            ValidationError = ValidateForm();
            if(ValidationError is not null || _topicId is null)
                return;
            _topicListener.OnMessageReceived -= MessageReceived;
            _topicListener.Unsubscribe(_topicId.Value);
            Listening = false;
        });
    }

    private void MessageReceived(object? sender, IncomingMessageData data)
    {
        if(data.Topic == Topic)
            Dispatcher.UIThread.Post(() => Messages.Add(data));
    }

    private void BroadcastListenerUpdated()
    {
        MessageBus.Current.SendMessage(Listener, BusEvents.ListenerUpdated);
    }

    private string? ValidateForm()
    {
        var sb = new StringBuilder();
        if (string.IsNullOrWhiteSpace(Topic)) sb.AppendLine("Topic is empty");
        return sb.Length > 0 ? sb.ToString() : null;
    }

    public void Dispose()
    {
        _topicListener.OnMessageReceived -= MessageReceived;
    }
}