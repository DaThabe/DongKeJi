using System.Windows;
using DongKeJi.Inject;
using Microsoft.Extensions.DependencyInjection;
using Wpf.Ui;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace DongKeJi.Core.UI.View;


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

    private bool _isUserClosedPane;

    private bool _isPaneOpenedOrClosedFromCode;

    private void MainWindow_OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (_isUserClosedPane) return;

        _isPaneOpenedOrClosedFromCode = true;
        RootNavigation.IsPaneOpen = e.NewSize.Width > 800;
        _isPaneOpenedOrClosedFromCode = false;
    }

    private void NavigationView_OnPaneOpened(NavigationView sender, RoutedEventArgs args)
    {
        if (_isPaneOpenedOrClosedFromCode) return;

        _isUserClosedPane = false;
    }

    private void NavigationView_OnPaneClosed(NavigationView sender, RoutedEventArgs args)
    {
        if (_isPaneOpenedOrClosedFromCode) return;

        _isUserClosedPane = true;
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