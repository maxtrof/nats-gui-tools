﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Text;
using System.Windows.Input;
using Application.TopicListener;
using Autofac;
using Avalonia.Threading;
using Domain.Interfaces;
using Domain.Models;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using UI.Helpers;
using UI.MessagesBus;

namespace UI.ViewModels;

internal sealed class ListenerEditViewModel : ViewModelBase, IDisposable
{
    private readonly TopicListener _topicListener;
    private readonly IDataStorage _storage;

    private long? _topicId = null; 

    private string _name = default!;
    private string _topic = default!;
    private string? _validationError;
    private string _searchText = "";

    private SourceCache<IncomingMessageData, int> _messages = new (x => x.GetHashCode());
    private readonly ReadOnlyObservableCollection<IncomingMessageData> _messagesFiltered;
    public ReadOnlyObservableCollection<IncomingMessageData> Messages => _messagesFiltered;
    
    public readonly Guid ListenerId = default!;
    private bool _listening;

    /// <summary> Starts listening </summary>
    public ICommand StartListen { get; set; } = default!;
    /// <summary> Stops listening </summary>
    public ICommand StopListen { get; set; } = default!;
    /// <summary> Clears messages </summary>
    public ICommand ClearMessages { get; set; } = default!;
    
    public ObservableCollection<string> AutocompleteOptions => SharedObservables.Suggestions;

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

    public string SearchText
    {
        get => _searchText;
        set => this.RaiseAndSetIfChanged(ref _searchText, value);
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
        _storage = scope.Resolve<IDataStorage>();

        _messages.Connect()
            .AutoRefreshOnObservable(_ => this.ObservableForProperty(x => x.SearchText))
            .Filter(x =>
                string.IsNullOrWhiteSpace(SearchText) || x.Body.ToLower().Contains(SearchText.ToLower()))
            .Sort(SortExpressionComparer<IncomingMessageData>.Descending(t => t.Received))
            .Bind(out _messagesFiltered)
            .Subscribe();
        Init();
    }

    public ListenerEditViewModel(Listener listener)
    {
        var scope = Program.Container.BeginLifetimeScope();
        _topicListener = scope.Resolve<TopicListener>();
        _storage = scope.Resolve<IDataStorage>();
        ListenerId = listener.Id;
        Name = listener.Name;
        Topic = listener.Topic;

        _messages.Connect()
            .AutoRefreshOnObservable(_ => this.ObservableForProperty(x => x.SearchText))
            .Filter(x =>
                string.IsNullOrWhiteSpace(SearchText) || x.Body.ToLower().Contains(SearchText.ToLower()))
            .Sort(SortExpressionComparer<IncomingMessageData>.Descending(t => t.Received))
            .Bind(out _messagesFiltered)
            .Subscribe();

        var messages = _topicListener.GetMessages(listener.Topic);
        if (messages is not null)
        {
            foreach (var incomingMessageData in messages)
            {
                _messages.AddOrUpdate(incomingMessageData);
            }
        }

        Init();
    }

    private void Init()
    {
        _topicListener.OnUnsubscribed += OnUnsubscribe;
        StartListen = ReactiveCommand.Create<Unit>(_ =>
        {
            ValidationError = ValidateForm();
            if (ValidationError is not null)
                return;
            try
            {
                _topicId = _topicListener.SubscribeToListen(Topic);
                _topicListener.OnMessageReceived += MessageReceived;
                Listening = true;
            }
            catch (Exception ex)
            {
                ErrorHelper.ShowError(ex.Message);
            }
        });
        StopListen = ReactiveCommand.Create<Unit>(_ =>
        {
            ValidationError = ValidateForm();
            if (ValidationError is not null || _topicId is null)
                return;
            try
            {
                _topicListener.Unsubscribe(_topicId.Value);
                _topicListener.OnMessageReceived -= MessageReceived;
                Listening = false;
            }
            catch (Exception ex)
            {
                ErrorHelper.ShowError(ex.Message);
            }
        });
        ClearMessages = ReactiveCommand.Create<Unit>(_ =>
        {
            try
            {
                _topicListener.Clear(Topic);
                _messages.Clear();
            }
            catch (Exception ex)
            {
                ErrorHelper.ShowError(ex.Message);
            }
        });
    }

    private void MessageReceived(object? sender, IncomingMessageData data)
    {
        if (data.Topic == Topic && Listening)
        {
            var messageToAdd = _storage.AppSettings.FormatJson
                ? data with { Body = JsonFormatter.TryFormatJson(data.Body) }
                : data;
            Dispatcher.UIThread.Post(() => _messages.AddOrUpdate(messageToAdd));
        }
    }

    private void OnUnsubscribe(object? sender, string topicName)
    {
        if (topicName == Topic)
        {
            Listening = false;
            _topicListener.OnMessageReceived -= MessageReceived;
            _messages.Clear();
        }
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
        _topicListener.OnUnsubscribed -= OnUnsubscribe;
    }
}