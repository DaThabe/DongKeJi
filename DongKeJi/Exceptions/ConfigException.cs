namespace DongKeJi.Exceptions;

/// <summary>
/// 配置异常
/// </summary>
public class ConfigException : Exception
{

    public ConfigException(string message) : base(message)
    {

    }

    public ConfigException(string message, Exception innerException) : base(message, innerException)
    {

    }
}