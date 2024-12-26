using DongKeJi.Common.Inject;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using DongKeJi.ViewModel;
using Wpf.Ui.Controls;

namespace DongKeJi.Service;


/// <summary>
/// 主窗口服务
/// </summary>
public interface IMainFrameService
{
    /// <summary>
    /// 标题
    /// </summary>
    string Title { get; set; }

    /// <summary>
    /// 菜单
    /// </summary>
    ObservableCollection<NavigationViewItem> MenuItems { get; }

    /// <summary>
    /// 页脚菜单
    /// </summary>
    ObservableCollection<NavigationViewItem> FooterMenuItems { get; }
}


[Inject(ServiceLifetime.Singleton, typeof(IMainFrameService))]
internal class MainFrameService(IApplicationContext applicationContext) : IMainFrameService
{
    public string Title
    {
        get => applicationContext.MainFrame.Title;
        set => applicationContext.MainFrame.Title = value;
    }

    public ObservableCollection<NavigationViewItem> MenuItems
    {
        get =>  applicationContext.MainFrame.MenuItems;
        set => applicationContext.MainFrame.MenuItems = value;
    }

    public ObservableCollection<NavigationViewItem> FooterMenuItems
    {
        get => applicationContext.MainFrame.FooterMenuItems;
        set => applicationContext.MainFrame.FooterMenuItems = value;
    }
}


public static class MainFrameServiceExtensions
{
    /// <summary>
    /// 添加菜单
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
    /// 添加页脚菜单
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
    /// 添加子菜单 (但是好像最多只能一层子菜单
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
    /// 根据标题查询菜单
    /// </summary>
    /// <param name="vm"></param>
    /// <param name="title"></param>
    /// <returns></returns>
    public static NavigationViewItem? FindMenuByTitle(this IMainFrameService vm, string title)
    {
        return vm.MenuItems.FirstOrDefault(x => x.Content.ToString() == title);
    }

    /// <summary>
    /// 根据标题查询页脚菜单
    /// </summary>
    /// <param name="vm"></param>
    /// <param name="title"></param>
    /// <returns></returns>
    public static NavigationViewItem? FindFooterMenuByTitle(this IMainFrameService vm, string title)
    {
        return vm.FooterMenuItems.FirstOrDefault(x => x.Content.ToString() == title);
    }
}