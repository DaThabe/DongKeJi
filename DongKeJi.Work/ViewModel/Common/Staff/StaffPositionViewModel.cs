using CommunityToolkit.Mvvm.ComponentModel;
using DongKeJi.Common.ViewModel;
using DongKeJi.Work.Model.Entity.Staff;

namespace DongKeJi.Work.ViewModel.Common.Staff;

/// <summary>
///     员工职位
/// </summary>
public partial class StaffPositionViewModel : IdentifiableViewModel
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

    public static StaffPositionViewModel Empty { get; } = new()
    {
        Id = Guid.Empty
    };
}