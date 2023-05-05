namespace Domain.Models;

public sealed class NatsServerSettings
{
    /// <summary>
    /// Server name
    /// </summary>
    public string Name { get; set; } = default!;
    /// <summary>
    /// Server IP or name
    /// </summary>
    public string Address { get; set; } = default!;
    /// <summary>
    /// Port if provided
    /// </summary>
    public int? Port { get; set; }
    /// <summary>
    /// Username if provided
    /// </summary>
    public string? Login { get; set; }
    /// <summary>
    /// Password if provided
    /// </summary>
    public string? Password { get; set; }
    /// <summary>
    /// Should use secure connection
    /// </summary>
    public bool Tls { get; set; }
}