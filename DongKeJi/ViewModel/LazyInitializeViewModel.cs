namespace DongKeJi.ViewModel;

public interface ILazyInitialize
{
    /// <summary>
    /// 是否已经初始化
    /// </summary>
    bool IsInitialized { get; set; }

    /// <summary>
    /// 开始初始化
    /// </summary>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    ValueTask InitializeAsync(CancellationToken cancellation = default);
}


/// <summary>
///     延迟初始化视图模型
/// </summary>
public abstract class LazyInitializeViewModel : ObservableViewModel
{
    public bool IsInitialized { get; private set; }

    public async ValueTask InitializeAsync(CancellationToken cancellation = default)
    {
        if (IsInitialized) return;
        await OnInitializationAsync(cancellation);
        IsInitialized = true;
    }

    /// <summary>
    ///     开始初始化
    /// </summary>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    protected virtual ValueTask OnInitializationAsync(CancellationToken cancellation = default)
    {
        return ValueTask.CompletedTask;
    }
}