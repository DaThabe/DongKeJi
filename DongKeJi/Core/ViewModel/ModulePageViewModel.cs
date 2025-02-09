﻿using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using DongKeJi.Extensions;
using DongKeJi.Inject;
using DongKeJi.Module;
using DongKeJi.ViewModel;
using Microsoft.Extensions.DependencyInjection;

namespace DongKeJi.Core.ViewModel;


/// <summary>
///     设置
/// </summary>
[Inject(ServiceLifetime.Transient)]
public partial class ModulePageViewModel : ObservableViewModel
{
    /// <summary>
    ///     选中模块信息
    /// </summary>
    [ObservableProperty] 
    private IModuleInfo? _selectedModule;

    /// <summary>
    ///     所有模块
    /// </summary>
    [ObservableProperty] 
    private ObservableCollection<IModuleInfo> _moduleCollection = [];


    /// <summary>
    ///     设置
    /// </summary>
    public ModulePageViewModel()
    {
        ModuleCollection = ModuleExtensions.MetaInfos.ToObservableCollection();
        SelectedModule = Enumerable.FirstOrDefault<IModuleInfo>(ModuleCollection);
    }
}