namespace DongKeJi.Work.ViewModel.Common.Order;

/// <summary>
///     订单上下文
/// </summary>
public interface IOrderContext
{
    /// <summary>
    ///     当前订单
    /// </summary>
    OrderViewModel Order { get; set; }
}