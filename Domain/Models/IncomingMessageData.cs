namespace Domain.Models;

/// <summary>
/// Incoming to Nats topic message data
/// </summary>
/// <param name="Topic">Topic template, with wildcards</param>
/// <param name="SpecificTopic">Topic without wildcards</param>
public sealed record IncomingMessageData(
    string Topic,
    string SpecificTopic,
    string Body,
    string? TopicToReply,
    DateTime Received
)
{
    private string? _messagePreview;
    private const int PreviewMaxLength = 250;

    /// <summary>
    /// True if the message length > <see cref="PreviewMaxLength"/>
    /// </summary>
    public bool LargeMessage => Body.Length > PreviewMaxLength;
    /// <summary>
    /// Gets preview of the message Body
    /// </summary>
    public string Preview
    {
        get
        {
            if (!LargeMessage) return Body;
            if (_messagePreview is not null) return _messagePreview;
            
            var count = 0;
            var fifthLineIndex = 0;
            for (var i = 0; i < Body.Length; i++)
            {
                if (i >= PreviewMaxLength)
                {
                    fifthLineIndex = PreviewMaxLength;
                    break;
                }
                if (Body[i] != '\n') continue;
                count++;
                if (count != 5) continue;
                fifthLineIndex = i;
                break;
            }

            _messagePreview = Body[..fifthLineIndex] + "\n...";
            
            return _messagePreview;
        }
    }
}