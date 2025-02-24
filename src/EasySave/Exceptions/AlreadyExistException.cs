namespace EasySave.Exceptions;

public class AlreadyExistException(string message, string extension) : Exception(message)
{
    public string Extension { get; } = extension;
}