using DongKeJi.ViewModel.User;
using DongKeJi.Work.ViewModel.Common.Staff;

namespace DongKeJi.Work.ViewModel;

/// <summary>
///     员工面板上下文
/// </summary>
public interface IStaffDashboardContext :
    IUserContext,
    IStaffContext,
    IStaffPositionContext;