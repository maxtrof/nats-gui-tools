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
public class NatsGate : INatsGate
{
    /// <summary>
    /// Default timeout, 20 seconds
    /// </summary>
    private const int Timeout = 20 * 1000;
    
    private IConnection? _connection;

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
        await GetConnection().DrainAsync();
        GetConnection().Close();
        GetConnection().Dispose();
        _connection = null;
    }

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

    private IConnection GetConnection()
    {
        if (_connection is null) throw new NatsGateIsNotConnectedException("Please, connect to a server first", false);
        if (_connection.IsClosed()) throw new NatsGateIsNotConnectedException("Connection is closed", false);
        if (_connection.State != ConnState.CONNECTED) throw new NatsGateIsNotConnectedException("Connection is in a wrong state", true);
        return _connection;
    }
}