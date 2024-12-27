namespace DongKeJi.Work.ViewModel.Common.Staff;

/// <summary>
///     员工上下文
/// </summary>
public interface IStaffContext
{
    /// <summary>
    ///     员工
    /// </summary>
    StaffViewModel Staff { get; set; }
}

/// <summary>
/// 主员工上下文
/// </summary>
public interface IPrimaryStaffContext
{
    /// <summary>
    ///     主员工
    /// </summary>
    StaffViewModel PrimaryStaff { get; internal set; }
}