using System.Windows;
using DongKeJi.Common;
using DongKeJi.Common.Inject;
using DongKeJi.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Wpf.Ui;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace DongKeJi.UI.View;



[Inject(ServiceLifetime.Singleton)]
public partial class MainFrame : INavigationWindow
{
    private IServiceProvider Services { get; set; }


    public MainFrame(IServiceProvider services, IApplicationContext context)
    {
        InitializeComponent();

        SystemThemeWatcher.Watch(this);

        Services = services;
        DataContext = context.MainFrame;

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

    #region --事件--

    private bool _isUserClosedPane;

    private bool _isPaneOpenedOrClosedFromCode;

    private void MainWindow_OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (_isUserClosedPane)
        {
            return;
        }

        _isPaneOpenedOrClosedFromCode = true;
        RootNavigation.IsPaneOpen = e.NewSize.Width > 800;
        _isPaneOpenedOrClosedFromCode = false;
    }

    private void NavigationView_OnPaneOpened(NavigationView sender, RoutedEventArgs args)
    {
        if (_isPaneOpenedOrClosedFromCode)
        {
            return;
        }

        _isUserClosedPane = false;
    }

    private void NavigationView_OnPaneClosed(NavigationView sender, RoutedEventArgs args)
    {
        if (_isPaneOpenedOrClosedFromCode)
        {
            return;
        }

        _isUserClosedPane = true;
    }

    #endregion

    #region --导航--

    public INavigationView GetNavigation() => RootNavigation;

    public bool Navigate(Type pageType) => RootNavigation.Navigate(pageType);

    INavigationView INavigationWindow.GetNavigation() => RootNavigation;

    public void SetPageService(IPageService pageService) => RootNavigation.SetPageService(pageService);


    #endregion INavigationWindow methods

    #region --窗体--

    /// <summary>
    /// Raises the closed event.
    /// </summary>
    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);

        // Make sure that closing this window will begin the process of closing the application.
        Services.GetRequiredService<Application>().Shutdown();
    }

    public void ShowWindow() => Show();

    public void CloseWindow() => Close();

    public void SetServiceProvider(IServiceProvider serviceProvider)
    {
        Services = serviceProvider;
    }


    #endregion
}
