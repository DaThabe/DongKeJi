using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using DongKeJi.Extensions;
using DongKeJi.Inject;
using DongKeJi.Module;
using DongKeJi.ViewModel;
using Microsoft.Extensions.DependencyInjection;

namespace DongKeJi.Core.ViewModel.Module;


/// <summary>
///     设置
/// </summary>
[Inject(ServiceLifetime.Transient)]
public partial class ModuleDashboardViewModel : ObservableViewModel
{
    /// <summary>
    ///     选中模块信息
    /// </summary>
    [ObservableProperty] 
    private IModuleMetaInfo? _selectedModule;

    /// <summary>
    ///     所有模块
    /// </summary>
    [ObservableProperty] 
    private ObservableCollection<IModuleMetaInfo> _moduleCollection = [];


    /// <summary>
    ///     设置
    /// </summary>
    public ModuleDashboardViewModel()
    {
        ModuleCollection = ModuleExtensions.MetaInfos.ToObservableCollection();
        SelectedModule = ModuleCollection.FirstOrDefault();
    }
}