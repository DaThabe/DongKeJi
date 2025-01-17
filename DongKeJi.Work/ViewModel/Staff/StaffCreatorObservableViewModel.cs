using CommunityToolkit.Mvvm.ComponentModel;
using DongKeJi.Inject;
using DongKeJi.ViewModel;
using Microsoft.Extensions.DependencyInjection;

namespace DongKeJi.Work.ViewModel.Staff;


[Inject(ServiceLifetime.Transient)]
public partial class StaffCreatorObservableViewModel : ObservableViewModel
{
    /// <summary>
    ///     职位信息
    /// </summary>
    [ObservableProperty] private StaffViewModel _staff = new();
}