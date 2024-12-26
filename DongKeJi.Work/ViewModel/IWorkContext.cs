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
public interface IWorkContext : IUserContext, IStaffContext
{
}

[Inject(ServiceLifetime.Singleton, typeof(IWorkContext))]
public partial class WorkContext(

    IApplicationContext applicationContext
    
    ) : LazyInitializeViewModel, IWorkContext
{
    public UserViewModel User
    {
        get => applicationContext.User;
        set => applicationContext.User = value;
    }

    [ObservableProperty] private StaffViewModel _staff = StaffViewModel.Empty;
}