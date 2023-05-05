namespace Domain.Models;

/// <summary>
/// Incoming to Nats topic message data
/// </summary>
public sealed record IncomingMessageData(
    string Topic,
    string Body,
    string? TopicToReply
);