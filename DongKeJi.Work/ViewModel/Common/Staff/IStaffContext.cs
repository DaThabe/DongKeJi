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