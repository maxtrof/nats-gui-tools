namespace Application.Exceptions;

/// <summary>
/// Exception during import data
/// </summary>
public class ImportException : Exception
{
    public ImportException(string message) : base(message) {} 
}