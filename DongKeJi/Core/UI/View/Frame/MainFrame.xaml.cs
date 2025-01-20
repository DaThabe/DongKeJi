using System.DirectoryServices.ActiveDirectory;
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
    // 定义依赖属性
    public static readonly DependencyProperty SelectedNavigationViewItemProperty =
        DependencyProperty.Register(
            nameof(SelectedNavigationViewItem),
            typeof(INavigationViewItem), 
            typeof(TitleBar),
            new PropertyMetadata(null, (o, args) =>
            {
                if (o is not MainFrame mf) return;

                var s = args.NewValue;

                const bool isSelectedChildMenu = true;
                var fuck = mf.RootNavigation.MenuItems;
                foreach (var i in fuck)
                {
                    if (i is not NavigationViewItem item) continue;
                    var name = item.Content;
                    var f = item.IsFocused;
                    var a = item.IsActive;
                }

                foreach (var i in GetTopMenus())
                {
                    if (!i.IsActive) continue;

                    mf._isSelectedChildNavigationView = false;
                    break;
                }

                mf._isSelectedChildNavigationView = isSelectedChildMenu;
                mf.RootNavigation.IsPaneToggleVisible = !mf._isSelectedChildNavigationView;
                return;

                IEnumerable<NavigationViewItem> GetTopMenus()
                {
                    foreach (var i in mf.RootNavigation.MenuItems)
                    {
                        if (i is not NavigationViewItem nav) continue;
                        yield return nav;
                    }

                    foreach (var i in mf.RootNavigation.FooterMenuItems)
                    {
                        if (i is not NavigationViewItem nav) continue;
                        yield return nav;
                    }
                }
            }));

    // 封装的 CLR 属性
    public string SelectedNavigationViewItem
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

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


    #endregion

    #region --导航--

    public INavigationView GetNavigation()
    {
        return RootNavigation;
    }

    public bool Navigate(Type pageType)
    {
        var result =  RootNavigation.Navigate(pageType);
        if (!result) return false;

        foreach (var i in RootNavigation.MenuItems)
        {
            if (i is not INavigationViewItem item) continue;
            if (item.TargetPageType != pageType) continue;

            _isSelectedChildNavigationView = false;
            return true;
        }

        _isSelectedChildNavigationView = true;
        return true;
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