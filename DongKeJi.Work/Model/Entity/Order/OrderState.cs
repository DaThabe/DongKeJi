namespace DongKeJi.Work.Model.Entity.Order;

/// <summary>
///     订单状态
/// </summary>
public enum OrderState
{
    /// <summary>
    ///     没有状态
    /// </summary>
    None,

    /// <summary>
    ///     等待开始
    /// </summary>
    Ready,

    /// <summary>
    ///     进行中
    /// </summary>
    Active,

    /// <summary>
    ///     暂停
    /// </summary>
    Paused,

    /// <summary>
    ///     过期
    /// </summary>
    Expired,

    /// <summary>
    ///     取消
    /// </summary>
    Cancel
}