namespace Application.Exceptions;

public class FailedToSubscribeToTopicException : Exception
{
    public FailedToSubscribeToTopicException(string message): base(message){}
}