using System.Windows;
using DongKeJi.Inject;
using Microsoft.Extensions.DependencyInjection;
using Wpf.Ui;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace DongKeJi.Core.UI.View.Frame;


[Inject(ServiceLifetime.Singleton)]
partial class MainFrame : INavigationWindow
{
    public MainFrame(IServiceProvider services, ICoreModule module)
    {
        InitializeComponent();

        SystemThemeWatcher.Watch(this);

        Services = services;
        DataContext = module.MainFrame;

        //设置页面服务
        var pageService = services.GetRequiredService<IPageService>();
        SetPageService(pageService);

        //消息弹窗功能
        var snackbarService = services.GetRequiredService<ISnackbarService>();
        snackbarService.SetSnackbarPresenter(SnackbarPresenter);

        //导航
        var navigationService = services.GetRequiredService<INavigationService>();
        navigationService.SetNavigationControl(RootNavigation);

        //设置弹窗服务
        var contentDialogService = services.GetRequiredService<IContentDialogService>();
        contentDialogService.SetDialogHost(ContentPresenterForDialogs);
    }


    private IServiceProvider Services { get; set; }

    #region --事件--

    /// <summary>
    /// 是否选中了子菜单元素
    /// </summary>
    private bool _isSelectedChildNavigationView;

    private void MainWindow_OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (_isSelectedChildNavigationView)
        {
            RootNavigation.IsPaneToggleVisible = false;
            RootNavigation.IsPaneOpen = true;
            return;
        }

        RootNavigation.IsPaneToggleVisible = e.NewSize.Width > 800 && !_isSelectedChildNavigationView;
        RootNavigation.IsPaneOpen = e.NewSize.Width > 800;
    }

    /// <summary>
    /// 选中顶级导航会触发
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void RootNavigation_OnSelectionChanged(NavigationView sender, RoutedEventArgs args)
    {
        RootNavigation.IsPaneToggleVisible = true;
        _isSelectedChildNavigationView = false;
    }

    /// <summary>
    /// 选中所有导航都会触发
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void RootNavigation_OnNavigated(NavigationView sender, NavigatedEventArgs args)
    {
        var isSelectedChildMenu = true;

        foreach (var i in GetTopMenus())
        {
            if (!i.IsActive) continue;

            _isSelectedChildNavigationView = false;
            break;
        }

        _isSelectedChildNavigationView = isSelectedChildMenu;
        RootNavigation.IsPaneToggleVisible = !_isSelectedChildNavigationView;
        return;

        IEnumerable<NavigationViewItem> GetTopMenus()
        {
            foreach (var i in sender.MenuItems)
            {
                if (i is not NavigationViewItem nav) continue;
                yield return nav;
            }

            foreach (var i in sender.FooterMenuItems)
            {
                if (i is not NavigationViewItem nav) continue;
                yield return nav;
            }
        }
    }

    #endregion

    #region --导航--

    public INavigationView GetNavigation()
    {
        return RootNavigation;
    }

    public bool Navigate(Type pageType)
    {
        return RootNavigation.Navigate(pageType);
    }

    INavigationView INavigationWindow.GetNavigation()
    {
        return RootNavigation;
    }

    public void SetPageService(IPageService pageService)
    {
        RootNavigation.SetPageService(pageService);
    }

    #endregion INavigationWindow methods

    #region --窗体--

    /// <summary>
    ///     Raises the closed event.
    /// </summary>
    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);

        // Make sure that closing this window will begin the process of closing the application.
        Services.GetRequiredService<IApplication>().Shutdown();
    }

    public void ShowWindow()
    {
        Show();
    }

    public void CloseWindow()
    {
        Close();
    }

    public void SetServiceProvider(IServiceProvider serviceProvider)
    {
        Services = serviceProvider;
    }

    #endregion

    
}