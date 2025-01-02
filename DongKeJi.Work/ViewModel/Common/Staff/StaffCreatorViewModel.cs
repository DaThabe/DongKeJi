using CommunityToolkit.Mvvm.ComponentModel;
using DongKeJi.Common.Inject;
using DongKeJi.Common.ViewModel;
using Microsoft.Extensions.DependencyInjection;

namespace DongKeJi.Work.ViewModel.Common.Staff;


[Inject(ServiceLifetime.Transient)]
public partial class StaffCreatorViewModel : ViewModelBase, IStaffContext
{
    /// <summary>
    ///     职位信息
    /// </summary>
    [ObservableProperty] private StaffViewModel _staff = new();
}