using CommunityToolkit.Mvvm.ComponentModel;
using DongKeJi.Common.Inject;
using DongKeJi.Common.ViewModel;
using Microsoft.Extensions.DependencyInjection;

namespace DongKeJi.Work.ViewModel.Common.Staff;


[Inject(ServiceLifetime.Transient)]
public partial class StaffPositionCreatorViewModel : ViewModelBase, IStaffPositionContext
{
    /// <summary>
    ///     职位信息
    /// </summary>
    [ObservableProperty] private StaffPositionViewModel _position = StaffPositionViewModel.Empty;
}