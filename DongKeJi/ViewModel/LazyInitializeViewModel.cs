namespace DongKeJi.ViewModel;

/// <summary>
///     延迟初始化视图模型
/// </summary>
public abstract class LazyInitializeViewModel : ObservableViewModel
{
    private readonly object _initializationLock = new();

    public bool IsInitialized { get; private set; }

    public async ValueTask InitializeAsync(CancellationToken cancellation = default)
    {
        lock (_initializationLock)
        {
            if (IsInitialized) return;
            IsInitialized = true;
        }

        await OnInitializationAsync(cancellation);
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