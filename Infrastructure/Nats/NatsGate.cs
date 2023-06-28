using System.Text;
using System.Text.Json;
using Application.Exceptions;
using Domain.Interfaces;
using Domain.Models;
using NATS.Client;

namespace Infrastructure.Nats;

/// <summary>
/// Default implementation of <see cref="INatsGate"/>
/// </summary>
public sealed class NatsGate : INatsGate, IDisposable
{
    /// <summary>
    /// Default timeout, 20 seconds
    /// </summary>
    private const int Timeout = 20 * 1000;
    
    private IConnection? _connection;
    private readonly List<IAsyncSubscription> _subscriptions = new ();

    /// <inheritdoc />
    public void ConnectToServer(NatsServerSettings server)
    {
        var cf = new ConnectionFactory();
        var options = ConnectionFactory.GetDefaultOptions();
        if (server.Password is not null)
        {
            options.Password = server.Password;
            options.User = server.Login;
        }

        if (server.Tls)
            options.Secure = true;

        options.Name = server.Name;

        options.Servers = server.Port is null
            ? new[] { server.Address }
            : new[] { $"{server.Address}:{server.Port}" };

        _connection = cf.CreateConnection(options);
    }

    /// <inheritdoc />
    public async Task Disconnect()
    {
        foreach (var asyncSubscription in _subscriptions)
        {
            asyncSubscription.Dispose();
        }
        _subscriptions.Clear();
        await GetConnection().DrainAsync();
        _connection?.Close();
        _connection = null;
    }

    public bool Connected => _connection?.State == ConnState.CONNECTED;

    /// <inheritdoc />
    public void SendRequest(NatsRequest request)
    {
        GetConnection().Publish(request.Topic, Encoding.UTF8.GetBytes(request.Body));
    }

    /// <inheritdoc />
    public async Task<T> SendRequestReply<T>(NatsRequest request)
    {
        var req = await GetConnection().RequestAsync(request.Topic, Encoding.UTF8.GetBytes(request.Body), Timeout);
        if (req is null) throw new ArgumentNullException(nameof(req));
        var deserialized = JsonSerializer.Deserialize<T>(req.Data);
        if (deserialized is null) throw new ArgumentNullException(nameof(deserialized));
        return deserialized;
    }

    /// <inheritdoc />
    public async Task<string> SendRequestReply(NatsRequest request)
    {
        var req = await GetConnection().RequestAsync(request.Topic, Encoding.UTF8.GetBytes(request.Body), Timeout);
        if (req is null) throw new ArgumentNullException(nameof(req));
        return Encoding.UTF8.GetString(req.Data);
    }

    /// <inheritdoc />
    public long Subscribe(string topicName, Func<IncomingMessageData, ResponseMessageData?> handler)
    {
        EventHandler<MsgHandlerEventArgs> h = (sender, args) =>
        {
            var body = Encoding.UTF8.GetString(args.Message.Data);
            var response = handler.Invoke(
                new IncomingMessageData(topicName, body, string.IsNullOrWhiteSpace(args.Message.Reply) ? null : args.Message.Reply, DateTime.Now)
            );
            if (response is null) return;
            
            GetConnection().Publish(response.TopicToResponse ?? args.Message.Reply, Encoding.UTF8.GetBytes(response.Body));
        };
        var sub = GetConnection().SubscribeAsync(topicName, h);
        if (sub is null) throw new FailedToSubscribeToTopicException("Not connected to a server");
        _subscriptions.Add(sub);
        
        return sub.Sid;
    }

    /// <inheritdoc />
    public ConnectionStats GetConnectionStats()
    {
        var c = GetConnection();
        
        return new ConnectionStats(
            MessagesSent: c.Stats.OutMsgs,
            MessagesReceived: c.Stats.InMsgs,
            Reconnects: c.Stats.Reconnects,
            JetStreamAvailable: c.ServerInfo.JetStreamAvailable,
            SubscriptionsCount: c.SubscriptionCount
        );
    }
    
    public void Unsubscribe(long subscriptionId)
    {
        var sub = _subscriptions.Find(x => x.Sid == subscriptionId);
        if (sub is null) return;
        sub.Unsubscribe();
        _subscriptions.Remove(sub);
    }

    private IConnection GetConnection()
    {
        if (_connection is null) throw new NatsGateIsNotConnectedException("Please, connect to a server first", false);
        if (_connection.IsClosed()) throw new NatsGateIsNotConnectedException("Connection is closed", false);
        if (_connection.State != ConnState.CONNECTED) throw new NatsGateIsNotConnectedException("Connection is in a wrong state", true);
        return _connection;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        foreach (var asyncSubscription in _subscriptions)
        {
            asyncSubscription.Dispose();
        }

        _connection?.Dispose();
    }
}