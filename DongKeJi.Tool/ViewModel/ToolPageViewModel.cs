using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DongKeJi.Inject;
using DongKeJi.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace DongKeJi.Tool.ViewModel;


[Inject(ServiceLifetime.Singleton)]
public partial class ToolPageViewModel(IPageService pageService) : ObservableViewModel
{
    /// <summary>
    /// 当前工具视图
    /// </summary>
    [ObservableProperty] 
    private FrameworkElement? _currentToolView;

    /// <summary>
    /// 选中的菜单元素
    /// </summary>
    [ObservableProperty] 
    private ToolItemViewModel? _selectedToolItem;

    /// <summary>
    /// 菜单集合
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<ToolItemViewModel> _toolItemCollection = [];


    partial void OnSelectedToolItemChanged(ToolItemViewModel? value)
    {
        if (value is null) return;

        (CurrentToolView as INavigationAware)?.OnNavigatedFrom();

        CurrentToolView = pageService.GetPage(value.PageType);

        (CurrentToolView as INavigationAware)?.OnNavigatedTo();
    }
}