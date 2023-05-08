using Domain.Interfaces;
using Domain.Models;

namespace Application;

/// <summary>
/// Manages connections to Nats servers
/// </summary>
public sealed class ConnectionManager
{
    private readonly INatsGate _natsGate;
    private NatsServerSettings? _connectedServer;

    public ConnectionManager(INatsGate natsGate)
    {
        _natsGate = natsGate ?? throw new ArgumentNullException(nameof(natsGate));
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
    public async Task Connect(NatsServerSettings settings)
    {
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