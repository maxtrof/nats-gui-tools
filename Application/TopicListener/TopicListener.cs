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
    /// Handler that just save message data and does no replies
    /// </summary>
    private ResponseMessageData? ListeningHandler(IncomingMessageData messageData)
    {
        if (_messages.TryGetValue(messageData.Topic, out var topic))
        {
            topic.Add(messageData);
        }
        else
        {
            _messages.Add(messageData.Topic, new List<IncomingMessageData>
            {
                messageData
            });
        }

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