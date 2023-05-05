namespace Domain.Models;

/// <summary>
/// Nats request model for app processing
/// </summary>
public class NatsRequest
{
    /// <summary>
    /// Target topic
    /// </summary>
    public string Topic { get; set; } = default!;
    /// <summary>
    /// Request body
    /// </summary>
    public string Body { get; set; } = default!;
}