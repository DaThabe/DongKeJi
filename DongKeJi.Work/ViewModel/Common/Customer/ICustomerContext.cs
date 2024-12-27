namespace DongKeJi.Work.ViewModel.Common.Customer;

/// <summary>
///     订单上下文
/// </summary>
public interface ICustomerContext
{
    /// <summary>
    ///     当前机构
    /// </summary>
    CustomerViewModel Customer { get; set; }
}