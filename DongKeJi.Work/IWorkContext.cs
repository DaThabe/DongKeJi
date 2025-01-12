using CommunityToolkit.Mvvm.ComponentModel;
using DongKeJi.Inject;
using DongKeJi.ViewModel;
using DongKeJi.Work.ViewModel.Staff;
using Microsoft.Extensions.DependencyInjection;
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
internal partial class WorkContext : ObservableViewModel, IWorkContext
{
    [ObservableProperty] 
    private StaffViewModel? _currentStaff;
}