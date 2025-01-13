using CommunityToolkit.Mvvm.ComponentModel;
using DongKeJi.Core.ViewModel;
using DongKeJi.Core.ViewModel.Frame;
using DongKeJi.Core.ViewModel.User;
using DongKeJi.Inject;
using DongKeJi.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Wpf.Ui;

namespace DongKeJi.Core;

/// <summary>
///     主程序上下文
/// </summary>
public interface ICoreContext
{
    /// <summary>
    ///     主程序
    /// </summary>
    ApplicationViewModel Application { get; }

    /// <summary>
    ///     主窗口信息
    /// </summary>
    MainFrameViewModel MainFrame { get;  }

    /// <summary>
    /// 当前用户
    /// </summary>
    UserViewModel? CurrentUser { get;  }
}


[Inject(ServiceLifetime.Singleton, typeof(ICoreContext))]
internal partial class CoreContext(IThemeService themeService) : ObservableViewModel, ICoreContext
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
    [ObservableProperty] private UserViewModel? _currentUser;
}