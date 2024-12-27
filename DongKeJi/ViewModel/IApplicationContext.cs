using CommunityToolkit.Mvvm.ComponentModel;
using DongKeJi.Common.Inject;
using DongKeJi.Common.ViewModel;
using DongKeJi.ViewModel.Frame;
using DongKeJi.ViewModel.User;
using Microsoft.Extensions.DependencyInjection;
using Wpf.Ui;

namespace DongKeJi.ViewModel;

/// <summary>
///     主程序上下文
/// </summary>
public interface IApplicationContext
{
    /// <summary>
    ///     主程序
    /// </summary>
    ApplicationViewModel Application { get; internal set; }

    /// <summary>
    ///     窗口
    /// </summary>
    MainFrameViewModel MainFrame { get; internal set; }

    /// <summary>
    /// 当前登录用户
    /// </summary>
    UserViewModel LoginUser { get; internal set; }
}

[Inject(ServiceLifetime.Singleton, typeof(IApplicationContext))]
public partial class ApplicationContext(IThemeService themeService) : ViewModelBase, IApplicationContext
{
    /// <summary>
    ///     程序
    /// </summary>
    [ObservableProperty] private ApplicationViewModel _application = new(themeService);

    /// <summary>
    ///     主窗口
    /// </summary>
    [ObservableProperty] private MainFrameViewModel _mainFrame = new();

    /// <summary>
    ///     当前用户
    /// </summary>
    [ObservableProperty] private UserViewModel _loginUser = new();
}