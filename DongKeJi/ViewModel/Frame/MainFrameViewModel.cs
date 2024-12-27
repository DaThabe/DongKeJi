using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Wpf.Ui.Controls;

namespace DongKeJi.ViewModel.Frame;

/// <summary>
///     主窗口
/// </summary>
public partial class MainFrameViewModel : ObservableObject
{
    /// <summary>
    ///     页脚菜单
    /// </summary>
    [ObservableProperty] private ObservableCollection<NavigationViewItem> _footerMenuItems = [];

    /// <summary>
    ///     菜单元素
    /// </summary>
    [ObservableProperty] private ObservableCollection<NavigationViewItem> _menuItems = [];

    /// <summary>
    ///     标题
    /// </summary>
    [ObservableProperty] private string _title = string.Empty;
}