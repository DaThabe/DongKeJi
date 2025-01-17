using CommunityToolkit.Mvvm.ComponentModel;
using DongKeJi.Inject;
using DongKeJi.ViewModel;
using DongKeJi.Work.Model.Entity.Staff;
using Microsoft.Extensions.DependencyInjection;

namespace DongKeJi.Work.ViewModel.Staff;


[Inject(ServiceLifetime.Transient)]
public partial class StaffPositionCreatorViewModel : ObservableViewModel
{
    /// <summary>
    ///     职位信息
    /// </summary>
    [ObservableProperty] private StaffPositionViewModel _position = new()
    {
        Title = "新增职位",
        Type = StaffPositionType.Designer
    };
}