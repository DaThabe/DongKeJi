using CommunityToolkit.Mvvm.ComponentModel;
using DongKeJi.Common.ViewModel;
using DongKeJi.Work.ViewModel.Common.Customer;

namespace DongKeJi.Work.ViewModel.Common.Staff;

/// <summary>
///     员工
/// </summary>
public partial class StaffViewModel : IdentifiableViewModel, IEmptyable<StaffViewModel>
{
    /// <summary>
    ///     是否是主账户  (真实身份, 其他的员工只是用来填充数据
    /// </summary>
    [ObservableProperty] private bool _isPrimaryAccount;


    /// <summary>
    ///     用户名
    /// </summary>
    [ObservableProperty] private string _name = string.Empty;



    public bool IsEmpty => this == Empty;

    public static StaffViewModel Empty { get; } = new() { Id = Guid.Empty };
}