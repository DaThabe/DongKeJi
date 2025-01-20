namespace DongKeJi.UI;

/// <summary>
/// 导航状态
/// </summary>
public enum NavigationState
{
    /// <summary>
    /// 初始
    /// </summary>
    None = 0,

    /// <summary>
    /// 发生错误
    /// </summary>
    Error = 1,

    /// <summary>
    /// 进入中
    /// </summary>
    Entering = 100,

    /// <summary>
    /// 离开中
    /// </summary>
    Leaving = 101,

    /// <summary>
    /// 已经进入
    /// </summary>
    Entered = 200,

    /// <summary>
    /// 已经离开
    /// </summary>
    Leaved = 201
}