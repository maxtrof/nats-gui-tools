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
    private readonly MockEngine.MockEngine _mockEngine;
    private NatsServerSettings? _connectedServer;

    public ConnectionManager(INatsGate natsGate, TopicListener.TopicListener topicListener, MockEngine.MockEngine mockEngine)
    {
        _natsGate = natsGate ?? throw new ArgumentNullException(nameof(natsGate));
        _topicListener = topicListener ?? throw new ArgumentNullException(nameof(topicListener));
        _mockEngine = mockEngine ?? throw new ArgumentNullException(nameof(mockEngine));
    }

    /// <summary>
    /// Returns if there's active connection
    /// </summary>
    public bool IsConnected => _connectedServer is not null && _natsGate.Connected;

    /// <summary>
    /// Returns active connection stats
    /// </summary>
    public ConnectionStats GetStats => _natsGate.GetConnectionStats();

    public string GetCurrentServerName => IsConnected
        ? _connectedServer?.Address ?? ""
        : string.Empty;

    /// <summary>
    /// Connects to a server. If there's active connection - closes it and opens a new one
    /// </summary>
    /// <param name="settings">Server settings</param>
    /// <param name="clearTopicsAndMocks">Clears all topics</param>
    public async Task Connect(NatsServerSettings settings, bool clearTopicsAndMocks = true)
    {
        if (clearTopicsAndMocks)
        {
            _topicListener.UnsubscribeAll();
            _mockEngine.DeactivateAllRules();
        }
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
        if (!IsConnected) return;
        await _natsGate.Disconnect();
        _connectedServer = null;
    }
}