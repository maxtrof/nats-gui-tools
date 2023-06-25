using Domain.Models;

namespace Domain.Interfaces;

/// <summary>
/// Describes gate to interact with Nats
/// </summary>
public interface INatsGate
{
    /// <summary>
    /// Connects to a server
    /// </summary>
    /// <param name="server">Server</param>
    void ConnectToServer(NatsServerSettings server);

    /// <summary>
    /// Disconnect from a server
    /// </summary>
    Task Disconnect();
    
    /// <summary>
    /// True if connection is active
    /// </summary>
    bool Connected { get; }

    /// <summary>
    /// Publishes request
    /// </summary>
    /// <param name="request">Request to publish</param>
    void SendRequest(NatsRequest request);

    /// <summary>
    /// Publishes request and wait for reply
    /// </summary>
    /// <param name="request">Request to publish</param>
    /// <typeparam name="T">Type to deserialize to</typeparam>
    /// <returns>Deserialized reply</returns>
    Task<T> SendRequestReply<T>(NatsRequest request);

    /// <summary>
    /// Publishes request and waiting for reply
    /// </summary>
    /// <param name="request">Request to publish</param>
    /// <returns>Reply as string</returns>
    Task<string> SendRequestReply(NatsRequest request);

    /// <summary>
    /// Subscribes to the subject
    /// </summary>
    /// <param name="topicName">Subject</param>
    /// <param name="handler">Handler for incoming messages</param>
    /// <returns>Subscription Id</returns>
    long Subscribe(string topicName, Func<IncomingMessageData, ResponseMessageData?> handler);

    /// <summary>
    /// Unsubscribes from a subscription
    /// </summary>
    /// <param name="subscriptionId">Id of the subscription</param>
    void Unsubscribe(long subscriptionId);

    /// <summary>
    /// Gets active connection stats
    /// </summary>
    ConnectionStats GetConnectionStats();
}