using CommunityToolkit.Mvvm.ComponentModel;
using DongKeJi.Common.Inject;
using DongKeJi.Common.ViewModel;
using Microsoft.Extensions.DependencyInjection;

namespace DongKeJi.ViewModel.Setting;


/// <summary>
/// 设置
/// </summary>
[Inject(ServiceLifetime.Transient)]
public partial class SettingDashboardViewModel : LazyInitializeViewModel
{
    /// <summary>
    /// 程序Vm
    /// </summary>
    [ObservableProperty]
    private ApplicationViewModel _application;


    /// <summary>
    /// 设置
    /// </summary>
    public SettingDashboardViewModel(IServiceProvider services)
    {
        var applicationContext = services.GetRequiredService<IApplicationContext>();
        Application = applicationContext.Application;
    }
}
