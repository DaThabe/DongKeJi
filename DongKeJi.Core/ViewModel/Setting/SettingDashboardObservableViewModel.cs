using CommunityToolkit.Mvvm.ComponentModel;
using DongKeJi.Inject;
using DongKeJi.ViewModel;
using Microsoft.Extensions.DependencyInjection;

namespace DongKeJi.Core.ViewModel.Setting;


/// <summary>
///     设置
/// </summary>
[Inject(ServiceLifetime.Transient)]
public partial class SettingDashboardObservableViewModel : LazyInitializeObservableViewModel
{
    /// <summary>
    ///     程序Vm
    /// </summary>
    [ObservableProperty] private ApplicationViewModel _application;


    /// <summary>
    ///     设置
    /// </summary>
    public SettingDashboardObservableViewModel(IServiceProvider services)
    {
        var applicationContext = services.GetRequiredService<ICoreContext>();
        Application = applicationContext.Application;
    }
}