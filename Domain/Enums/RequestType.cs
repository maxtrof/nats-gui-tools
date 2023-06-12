namespace Domain.Enums;

/// <summary>
/// Request types
/// </summary>
public enum RequestType 
{
    /// <summary>
    /// Publish message, don't wait for reply
    /// </summary>
    Publish = 0,
    /// <summary>
    /// Publish message and wait for reply
    /// </summary>
    RequestReply = 1
}