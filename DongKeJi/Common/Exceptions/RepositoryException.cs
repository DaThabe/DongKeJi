namespace DongKeJi.Common.Exceptions;

/// <summary>
/// 储存库异常
/// </summary>
public class RepositoryException : Exception
{

    public RepositoryException(string message) : base(message)
    {

    }

    public RepositoryException(string message, Exception innerException) : base(message, innerException)
    {

    }
}