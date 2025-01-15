using System.Collections.ObjectModel;
using DongKeJi.Core.UI.View;
using DongKeJi.Inject;
using Microsoft.Extensions.DependencyInjection;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace DongKeJi.Core.Service;

/// <summary>
///     主窗口服务
/// </summary>
public interface IMainFrameService : INavigationWindow
{
    /// <summary>
    ///     标题
    /// </summary>
    string Title { get; set; }

    /// <summary>
    ///     菜单
    /// </summary>
    ObservableCollection<NavigationViewItem> MenuItems { get; }

    /// <summary>
    ///     页脚菜单
    /// </summary>
    ObservableCollection<NavigationViewItem> FooterMenuItems { get; }
}


[Inject(ServiceLifetime.Singleton, typeof(IMainFrameService))]
internal class MainFrameService(MainFrame mainFrame, ICoreModule coreModule) : IMainFrameService
{
    public string Title
    {
        get => coreModule.MainFrame.Title;
        set => coreModule.MainFrame.Title = value;
    }

    public ObservableCollection<NavigationViewItem> MenuItems
    {
        get => coreModule.MainFrame.MenuItems;
        set => coreModule.MainFrame.MenuItems = value;
    }

    public ObservableCollection<NavigationViewItem> FooterMenuItems
    {
        get => coreModule.MainFrame.FooterMenuItems;
        set => coreModule.MainFrame.FooterMenuItems = value;
    }


    public INavigationView GetNavigation() => mainFrame.GetNavigation();

    public bool Navigate(Type pageType) => mainFrame.Navigate(pageType);

    public void SetServiceProvider(IServiceProvider serviceProvider) => mainFrame.SetServiceProvider(serviceProvider);

    public void SetPageService(IPageService pageService) => mainFrame.SetPageService(pageService);

    public void ShowWindow() => mainFrame.ShowWindow();

    public void CloseWindow() => mainFrame.CloseWindow();
}

public static class MainFrameServiceExtensions
{
    /// <summary>
    ///     添加菜单
    /// </summary>
    /// <typeparam name="TPage"></typeparam>
    /// <param name="vm"></param>
    /// <param name="icon"></param>
    /// <param name="title"></param>
    /// <returns></returns>
    public static NavigationViewItem AddMenu<TPage>(this IMainFrameService vm, SymbolRegular icon, string title)
    {
        NavigationViewItem menu = new(title, icon, typeof(TPage));
        vm.MenuItems.Add(menu);

        return menu;
    }

    /// <summary>
    ///     添加页脚菜单
    /// </summary>
    /// <typeparam name="TPage"></typeparam>
    /// <param name="vm"></param>
    /// <param name="icon"></param>
    /// <param name="title"></param>
    /// <returns></returns>
    public static NavigationViewItem AddFooterMenu<TPage>(this IMainFrameService vm, SymbolRegular icon, string title)
    {
        NavigationViewItem menu = new(title, icon, typeof(TPage));
        vm.FooterMenuItems.Add(menu);

        return menu;
    }

    /// <summary>
    ///     添加子菜单 (但是好像最多只能一层子菜单
    /// </summary>
    /// <typeparam name="TPage"></typeparam>
    /// <param name="menu"></param>
    /// <param name="icon"></param>
    /// <param name="title"></param>
    /// <returns></returns>
    public static NavigationViewItem AddChildMenu<TPage>(this NavigationViewItem menu, SymbolRegular icon, string title)
    {
        NavigationViewItem childMenu = new(title, icon, typeof(TPage));
        menu.MenuItems.Add(childMenu);

        return childMenu;
    }


    /// <summary>
    ///     添加菜单
    /// </summary>
    /// <typeparam name="TPage"></typeparam>
    /// <param name="vm"></param>
    /// <param name="index"></param>
    /// <param name="icon"></param>
    /// <param name="title"></param>
    /// <returns></returns>
    public static NavigationViewItem InsertMenu<TPage>(this IMainFrameService vm, int index, SymbolRegular icon, string title)
    {
        NavigationViewItem menu = new(title, icon, typeof(TPage));
        vm.MenuItems.Insert(index, menu);

        return menu;
    }

    /// <summary>
    ///     添加页脚菜单
    /// </summary>
    /// <typeparam name="TPage"></typeparam>
    /// <param name="vm"></param>
    /// <param name="index"></param>
    /// <param name="icon"></param>
    /// <param name="title"></param>
    /// <returns></returns>
    public static NavigationViewItem InsertFooterMenu<TPage>(this IMainFrameService vm, int index, SymbolRegular icon, string title)
    {
        NavigationViewItem menu = new(title, icon, typeof(TPage));
        vm.FooterMenuItems.Insert(index, menu);

        return menu;
    }




    /// <summary>
    ///     根据标题查询菜单
    /// </summary>
    /// <param name="vm"></param>
    /// <param name="title"></param>
    /// <returns></returns>
    public static NavigationViewItem? FindMenuByTitle(this IMainFrameService vm, string title)
    {
        return vm.MenuItems.FirstOrDefault(x => x.Content.ToString() == title);
    }

    /// <summary>
    ///     根据标题查询页脚菜单
    /// </summary>
    /// <param name="vm"></param>
    /// <param name="title"></param>
    /// <returns></returns>
    public static NavigationViewItem? FindFooterMenuByTitle(this IMainFrameService vm, string title)
    {
        return vm.FooterMenuItems.FirstOrDefault(x => x.Content.ToString() == title);
    }
}