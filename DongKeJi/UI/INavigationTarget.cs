namespace DongKeJi.UI;

/// <summary>
/// 表示被导航的目标
/// </summary>
public interface INavigationTarget
{
    /// <summary>
    /// 导航进入
    /// </summary>
    /// <param name="parameter">导航参数，提供进入页面时所需的上下文信息。</param>
    /// <param name="cancellation">取消令牌，用于取消操作。</param>
    /// <returns>异步操作任务。</returns>
    ValueTask NavigateToAsync(object? parameter, CancellationToken cancellation = default);

    /// <summary>
    /// 导航离开
    /// </summary>
    /// <param name="cancellation">取消令牌，用于取消操作。</param>
    /// <returns>异步操作任务。</returns>
    ValueTask NavigateFromAsync(CancellationToken cancellation = default);
}