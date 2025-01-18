using CommunityToolkit.Mvvm.ComponentModel;
using DongKeJi.ViewModel;
using DongKeJi.WebView.Model;
using Wpf.Ui.Controls;

namespace DongKeJi.WebView.ViewModel;

public partial class PageViewModel(IconElement icon, string title, Uri source) : EntityViewModel, IPage
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
    /// 页面网址
    /// </summary>
    [ObservableProperty] 
    private Uri _source = source;


    public override string ToString() => Title;
}