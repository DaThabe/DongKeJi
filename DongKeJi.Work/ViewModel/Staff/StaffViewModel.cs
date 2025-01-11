using CommunityToolkit.Mvvm.ComponentModel;
using DongKeJi.ViewModel;

namespace DongKeJi.Work.ViewModel.Staff;

/// <summary>
///     员工
/// </summary>
public partial class StaffViewModel : EntityViewModel, IWorkEntityViewModel
{
    /// <summary>
    ///     用户名
    /// </summary>
    [ObservableProperty] 
    private string _name = string.Empty;
}