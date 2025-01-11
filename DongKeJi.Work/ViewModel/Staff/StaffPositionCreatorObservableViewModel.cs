using CommunityToolkit.Mvvm.ComponentModel;
using DongKeJi.ViewModel;
using Microsoft.Extensions.DependencyInjection;

namespace DongKeJi.Work.ViewModel.Staff;


[Inject(ServiceLifetime.Transient)]
public partial class StaffPositionCreatorObservableViewModel : DongKeJi.ViewModel.ObservableViewModel, IStaffPositionContext
{
    /// <summary>
    ///     职位信息
    /// </summary>
    [ObservableProperty] private StaffPositionViewModel _position = StaffPositionViewModel.Default;
}