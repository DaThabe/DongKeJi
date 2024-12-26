namespace DongKeJi.Work.ViewModel.Common.Staff;

/// <summary>
/// 员工职位上下文
/// </summary>
public interface IStaffPositionContext
{
    /// <summary>
    ///     员工职位
    /// </summary>
    StaffPositionViewModel Position { get; set; }
}