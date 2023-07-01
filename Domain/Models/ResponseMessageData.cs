namespace Domain.Models;

/// <summary>
/// Response to a message via subscription
/// </summary>
/// <param name="Body">Body of a message</param>
/// <param name="TopicToResponse">Topic to send response message</param>
public sealed record ResponseMessageData(
    string Body,
    string? TopicToResponse
);