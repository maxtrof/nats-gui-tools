using Application.Exceptions;
using Domain.Interfaces;
using Domain.Models;

namespace Application.TopicListener;

/// <summary>
/// Provides tools for listening topics
/// </summary>
public sealed class TopicListener : IDisposable
{
    private readonly INatsGate _natsGate;
    
    /// <summary>
    /// Contains pairs of Topics and Received messages
    /// </summary>
    private readonly Dictionary<string, List<IncomingMessageData>> _messages = new();
    /// <summary>
    /// Pairs of topic names and subscriptions Ids
    /// </summary>
    private readonly Dictionary<string, long> _subscriptions = new();

    public TopicListener(INatsGate natsGate)
    {
        _natsGate = natsGate;
    }
    
    /// <summary>
    /// Event, firing on message received
    /// </summary>
    public event EventHandler<IncomingMessageData> OnMessageReceived;

    /// <summary>
    /// Event, firing on disconnect, passing topic name
    /// </summary>
    public event EventHandler<string> OnUnsubscribed;

    /// <summary>
    /// Gets messages or returns null
    /// </summary>
    /// <param name="topicName">Subject name</param>
    /// <returns>Messages</returns>
    public List<IncomingMessageData>? GetMessages(string topicName)
    {
        return !_messages.TryGetValue(topicName, out var messages) 
            ? null 
            : messages;
    }

    /// <summary>
    /// Subscribes to a topic to listen
    /// </summary>
    /// <param name="topicName">Subject name</param>
    /// <returns>Subscription Id</returns>
    public long SubscribeToListen(string topicName)
    {
        if (_subscriptions.ContainsKey(topicName)) throw new AlreadyListeningToATopicException(topicName);
        var subId = _natsGate.Subscribe(topicName, ListeningHandler);
        _subscriptions.Add(topicName, subId);
        return subId;
    }

    /// <summary>
    /// Clears messages from given topic
    /// </summary>
    /// <param name="topicName">Topic to clear</param>
    public void Clear(string topicName)
    {
        if (!_subscriptions.ContainsKey(topicName)) return;
        _messages[topicName].Clear();
    }

    /// <summary>
    /// Clears all messages from all topics
    /// </summary>
    public void ClearAll()
    {
        _messages.Clear();
    }

    /// <summary>
    /// Unsubscribes from listening topic by name
    /// </summary>
    /// <param name="topicName">Subject name</param>
    public void Unsubscribe(string topicName)
    {
        if (_subscriptions.TryGetValue(topicName, out var sub))
        {
            _natsGate.Unsubscribe(sub);
            _subscriptions.Remove(topicName);
            _messages.Remove(topicName);
            OnUnsubscribed.Invoke(this, topicName);
        }
    }
    
    /// <summary>
    /// Unsubscribes from listening topic by Id
    /// </summary>
    /// <param name="subscriptionId">Subject name</param>
    public void Unsubscribe(long subscriptionId)
    {
        var sub = _subscriptions.Single(x => x.Value == subscriptionId);
        Unsubscribe(sub.Key);
    }

    /// <summary>
    /// Unsubscribes from all topics and clears all messages
    /// </summary>
    public void UnsubscribeAll()
    {
        foreach (var (name, id) in _subscriptions)
        {
            _natsGate.Unsubscribe(id);
            _messages.Remove(name);
            OnUnsubscribed.Invoke(this, name);
        }
        _subscriptions.Clear();
    }

    /// <summary>
    /// Handler that just save message data and does no replies
    /// </summary>
    private ResponseMessageData? ListeningHandler(IncomingMessageData messageData)
    {
        if (_messages.TryGetValue(messageData.Topic, out var topic))
        {
            topic.Insert(0, messageData);
        }
        else
        {
            _messages.Add(messageData.Topic, new List<IncomingMessageData>
            {
                messageData
            });
        }

        OnMessageReceived.Invoke(this, messageData);
        return null;
    }

    public void Dispose()
    {
        foreach (var subscription in _subscriptions)
        {
            _natsGate.Unsubscribe(subscription.Value);
        }
    }
}