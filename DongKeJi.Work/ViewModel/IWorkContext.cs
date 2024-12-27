using CommunityToolkit.Mvvm.ComponentModel;
using DongKeJi.Common.Inject;
using DongKeJi.Common.ViewModel;
using DongKeJi.ViewModel;
using DongKeJi.ViewModel.User;
using DongKeJi.Work.ViewModel.Common.Staff;
using Microsoft.Extensions.DependencyInjection;

namespace DongKeJi.Work.ViewModel;

/// <summary>
///     办公上下文
/// </summary>
public interface IWorkContext : ILoginUserContext, IPrimaryStaffContext;


[Inject(ServiceLifetime.Singleton, typeof(IWorkContext))]
public partial class WorkContext(
    IApplicationContext applicationContext
) : LazyInitializeViewModel, IWorkContext
{
    [ObservableProperty] private StaffViewModel _primaryStaff = StaffViewModel.Empty;

    public UserViewModel LoginUser => applicationContext.LoginUser;
}