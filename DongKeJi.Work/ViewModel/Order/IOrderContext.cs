using DongKeJi.Work.ViewModel.Common.Order;

namespace DongKeJi.Work.ViewModel.Order;

/// <summary>
///     订单上下文
/// </summary>
public interface IOrderContext
{
    /// <summary>
    ///     当前订单
    /// </summary>
    OrderViewModel SelectedOrder { get; set; }
}