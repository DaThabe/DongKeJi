using CommunityToolkit.Mvvm.ComponentModel;
using DongKeJi.ViewModel;
using Microsoft.Extensions.DependencyInjection;

namespace DongKeJi.Work.ViewModel.Staff;


[Inject(ServiceLifetime.Transient)]
public partial class StaffCreatorObservableViewModel : DongKeJi.ViewModel.ObservableViewModel, IStaffContext
{
    /// <summary>
    ///     职位信息
    /// </summary>
    [ObservableProperty] private StaffViewModel _staff = new();
}