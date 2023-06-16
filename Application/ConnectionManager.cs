using Domain.Interfaces;
using Domain.Models;

namespace Application;

/// <summary>
/// Manages connections to Nats servers
/// </summary>
public sealed class ConnectionManager
{
    private readonly INatsGate _natsGate;
    private readonly TopicListener.TopicListener _topicListener;
    private NatsServerSettings? _connectedServer;

    public ConnectionManager(INatsGate natsGate, TopicListener.TopicListener topicListener)
    {
        _natsGate = natsGate ?? throw new ArgumentNullException(nameof(natsGate));
        _topicListener = topicListener;
    }

    /// <summary>
    /// Returns if there's active connection
    /// </summary>
    public bool IsConnected => _connectedServer is not null && _natsGate.Connected;

    public string GetCurrentServerName => IsConnected
        ? _connectedServer?.Address ?? ""
        : string.Empty;

    /// <summary>
    /// Connects to a server. If there's active connection - closes it and opens a new one
    /// </summary>
    /// <param name="settings">Server settings</param>
    /// <param name="clearTopics">Clears all topics</param>
    public async Task Connect(NatsServerSettings settings, bool clearTopics = true)
    {
        if (clearTopics)
            _topicListener.UnsubscribeAll();
        if (_natsGate.Connected)
            await _natsGate.Disconnect();
        _natsGate.ConnectToServer(settings);
        _connectedServer = settings;
    }

    /// <summary>
    /// Disconnects from a server.
    /// </summary>
    public async Task Disconnect()
    {
        await _natsGate.Disconnect();
        _connectedServer = null;
    }
}