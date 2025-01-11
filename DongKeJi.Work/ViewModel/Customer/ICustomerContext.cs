using DongKeJi.Work.ViewModel.Common.Customer;

namespace DongKeJi.Work.ViewModel.Customer;

/// <summary>
///     订单上下文
/// </summary>
public interface ICustomerContext
{
    /// <summary>
    ///     当前机构
    /// </summary>
    CustomerViewModel SelectedCustomer { get; set; }
}