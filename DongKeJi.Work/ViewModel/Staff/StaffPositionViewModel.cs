using CommunityToolkit.Mvvm.ComponentModel;
using DongKeJi.ViewModel;
using DongKeJi.Work.Model.Entity.Staff;

namespace DongKeJi.Work.ViewModel.Staff;

/// <summary>
///     员工职位
/// </summary>
public partial class StaffPositionViewModel : EntityViewModel, IWorkEntityViewModel
{
    /// <summary>
    ///     描述
    /// </summary>
    [ObservableProperty] private string? _describe;

    /// <summary>
    ///     标题
    /// </summary>
    [ObservableProperty] private string _title = string.Empty;

    /// <summary>
    ///     类型
    /// </summary>
    [ObservableProperty] private StaffPositionType _type = StaffPositionType.None;
}