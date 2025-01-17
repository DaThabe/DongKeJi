namespace DongKeJi.Work.Model.Entity.Order;

/// <summary>
///     订单状态
/// </summary>
public enum OrderState
{
    /// <summary>
    ///     没有状态
    /// </summary>
    None = 0,

    /// <summary>
    ///     等待开始
    /// </summary>
    Ready = 100,

    /// <summary>
    ///     进行中
    /// </summary>
    Active = 200,

    /// <summary>
    ///     暂停
    /// </summary>
    Paused = 300,

    /// <summary>
    ///     过期
    /// </summary>
    Expired = 400,

    /// <summary>
    ///     取消
    /// </summary>
    Cancel = 500
}