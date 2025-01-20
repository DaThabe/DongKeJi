namespace DongKeJi.UI;

/// <summary>
/// 表示一个导航服务
/// </summary>
public interface INavigationService
{
    /// <summary>
    /// 导航
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    ValueTask NavigationAsync(object key);

    /// <summary>
    /// 返回
    /// </summary>
    /// <returns></returns>
    ValueTask BackAsync();
}