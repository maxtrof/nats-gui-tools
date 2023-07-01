namespace Application.Exceptions;

public class DataStorageIsNotInitializedException : Exception
{
    public DataStorageIsNotInitializedException(string? message) : base(message){}
}