using Domain.Enums;

namespace Domain.Models;

/// <summary>
/// Template for a Request or Request/Reply to NATS topic
/// </summary>
public sealed class RequestTemplate
{
    /// <summary>
    /// Id
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

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

    public RequestType Type { get; set; } = RequestType.Publish;
}