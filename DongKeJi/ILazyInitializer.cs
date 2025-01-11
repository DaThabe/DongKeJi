namespace DongKeJi;

/// <summary>
///     表示一个可以延迟初始化的对象
/// </summary>
public interface ILazyInitializer
{
    /// <summary>
    ///     是否已经初始化
    /// </summary>
    bool IsInitialized { get; }

    /// <summary>
    ///     手动调用初始化
    /// </summary>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    Task InitializeAsync(CancellationToken cancellation = default);
}