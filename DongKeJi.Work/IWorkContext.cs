using CommunityToolkit.Mvvm.ComponentModel;
using DongKeJi.Inject;
using DongKeJi.ViewModel;
using DongKeJi.Work.ViewModel.Common.Staff;
using Microsoft.Extensions.DependencyInjection;
using StaffViewModel = DongKeJi.Work.ViewModel.Staff.StaffViewModel;

namespace DongKeJi.Work;

/// <summary>
///     办公上下文
/// </summary>
public interface IWorkContext
{
    /// <summary>
    /// 当前员工
    /// </summary>
    StaffViewModel? CurrentStaff { get; internal set; }
}


[Inject(ServiceLifetime.Singleton, typeof(IWorkContext))]
internal partial class WorkContext : DongKeJi.ViewModel.ObservableViewModel, IWorkContext
{
    [ObservableProperty] 
    private StaffViewModel? _currentStaff;
}