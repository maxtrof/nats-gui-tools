namespace Application.Exceptions;

/// <summary>
/// Exception thrown if Nats connection is not connected
/// </summary>
public class NatsGateIsNotConnectedException : Exception
{
    /// <summary>
    /// True if can wait for reconnection, False if should reconnect to a server
    /// </summary>
    public bool ShouldWaitForReconnection { get; set; }

    public NatsGateIsNotConnectedException(string message, bool shouldWaitForReconnection) : base(message)
    {
        ShouldWaitForReconnection = shouldWaitForReconnection;
    }
}