namespace Application.Exceptions;

public class AlreadyListeningToATopicException : Exception
{
    public AlreadyListeningToATopicException(string topic): base($"Already listening to a topic {topic}"){}
}