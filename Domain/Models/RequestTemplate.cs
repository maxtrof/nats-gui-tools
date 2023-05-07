namespace Domain.Models;

/// <summary>
/// Template for a Request or Request/Reply to NATS topic
/// </summary>
public sealed class RequestTemplate
{
    /// <summary>
    /// Template name
    /// </summary>
    public string Name { get; set; } = default!;
    /// <summary>
    /// Topic to send Request
    /// </summary>
    public string Topic { get; set; } = default!;
    /// <summary>
    /// Request body
    /// </summary>
    public string Body { get; set; } = default!;
}