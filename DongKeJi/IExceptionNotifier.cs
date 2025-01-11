namespace DongKeJi;

/// <summary>
///     提供异常通知功能
/// </summary>
public interface IExceptionCallback
{
    /// <summary>
    ///     异常回调
    /// </summary>
    event Action<Exception>? ExceptionCallback;
}