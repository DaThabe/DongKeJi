using CommunityToolkit.Mvvm.ComponentModel;
using DongKeJi.Inject;
using DongKeJi.ViewModel;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Wpf.Ui.Controls;

namespace DongKeJi.Core.ViewModel.Setting;


/// <summary>
///     设置
/// </summary>
[Inject(ServiceLifetime.Transient)]
public partial class SettingDashboardViewModel : LazyInitializeViewModel
{
    /// <summary>
    ///     程序Vm
    /// </summary>
    [ObservableProperty] private ApplicationViewModel _application;


    /// <summary>
    ///     设置
    /// </summary>
    public SettingDashboardViewModel(IServiceProvider services)
    {
        var applicationContext = services.GetRequiredService<ICoreContext>();
        Application = applicationContext.Application;
    }
}