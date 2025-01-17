using CommunityToolkit.Mvvm.ComponentModel;
using DongKeJi.Tool.Model;
using DongKeJi.ViewModel;
using Wpf.Ui.Controls;

namespace DongKeJi.Tool.ViewModel;

public partial class ToolItemViewModel(IconElement icon, string title, Type pageType) : 
    ObservableViewModel, IToolItem
{
    /// <summary>
    /// 图标
    /// </summary>
    [ObservableProperty] 
    private IconElement _icon = icon;

    /// <summary>
    /// 标题
    /// </summary>
    [ObservableProperty] 
    private string _title = title;

    /// <summary>
    /// 页面类型
    /// </summary>
    [ObservableProperty] 
    private Type _pageType = pageType;


    public override string ToString() => Title;
}