namespace DongKeJi.Common.ViewModel;

/// <summary>
/// 延迟初始化视图模型
/// </summary>
public abstract class LazyInitializeViewModel : ViewModelBase, ILazyInitializer
{
    private readonly object _initializationLock = new();

    public bool IsInitialized { get; private set; }

    public async Task InitializeAsync(CancellationToken cancellation = default)
    {
        lock (_initializationLock)
        {
            if (IsInitialized) return;
            IsInitialized = true;
        }

        await OnInitializationAsync(cancellation);
    }

    /// <summary>
    /// 开始初始化
    /// </summary>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    protected virtual Task OnInitializationAsync(CancellationToken cancellation = default)
    {
        return Task.CompletedTask;
    }
}