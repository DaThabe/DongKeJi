using DongKeJi.ViewModel.User;
using DongKeJi.Work.ViewModel.Common.Consume;
using DongKeJi.Work.ViewModel.Common.Customer;
using DongKeJi.Work.ViewModel.Common.Order;
using DongKeJi.Work.ViewModel.Common.Staff;

namespace DongKeJi.Work.ViewModel;

/// <summary>
///     明细管理上下文
/// </summary>
public interface IPerformanceDashboardContext :
    IUserContext,
    IStaffContext,
    ICustomerContext,
    IOrderContext,
    IConsumeContext;