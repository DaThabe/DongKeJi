using System.Collections.ObjectModel;
using DongKeJi.Core.UI.View.Frame;
using DongKeJi.Extensions;
using DongKeJi.Inject;
using DongKeJi.UI;
using Microsoft.Extensions.DependencyInjection;
using Wpf.Ui.Controls;

namespace DongKeJi.Core.Service;

/// <summary>
///     主窗口服务
/// </summary>
public interface IMainFrameService : INavigationService
{
    /// <summary>
    ///     标题
    /// </summary>
    string Title { get; set; }

    /// <summary>
    ///     菜单
    /// </summary>
    ObservableCollection<NavigationViewItem> MenuItems { get; set; }

    /// <summary>
    ///     页脚菜单
    /// </summary>
    ObservableCollection<NavigationViewItem> FooterMenuItems { get; set; }

    /// <summary>
    /// 显示窗口
    /// </summary>
    void Show();

    /// <summary>
    /// 关闭窗口
    /// </summary>
    void Close();
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

    public void Show() => mainFrame.Show();

    public void Close() => mainFrame.Close();


    public ValueTask NavigationAsync(object key)
    {
        if (key is Type pageType)
        {
            mainFrame.RootNavigation.Navigate(pageType);
        }

        return ValueTask.CompletedTask;
    }

    public ValueTask BackAsync()
    {
        mainFrame.RootNavigation.GoBack();
        return ValueTask.CompletedTask;
    }
}

public static class MainFrameServiceExtensions
{
    /// <summary>
    /// 添加菜单
    /// </summary>
    /// <param name="mainFrameService"></param>
    public static void AddMenu(this IMainFrameService mainFrameService, NavigationViewItem item)
    {
        mainFrameService.MenuItems.Add(item);
    }

    /// <summary>
    /// 添加页脚菜单
    /// </summary>
    /// <param name="mainFrameService"></param>
    /// <param name="item"></param>
    public static void AddFooterMenu(this IMainFrameService mainFrameService, NavigationViewItem item)
    {
        mainFrameService.FooterMenuItems.Add(item);
    }



    /// <summary>
    /// 添加菜单
    /// </summary>
    /// <typeparam name="TPage"></typeparam>
    /// <param name="mainFrameService"></param>
    /// <param name="icon"></param>
    /// <param name="title"></param>
    public static void AddMenu<TPage>(this IMainFrameService mainFrameService, SymbolRegular icon, string title)
    {
        var item = new NavigationViewItem(title, icon, typeof(TPage));
        mainFrameService.AddMenu(item);
    }

    /// <summary>
    /// 添加页脚菜单
    /// </summary>
    /// <typeparam name="TPage"></typeparam>
    /// <param name="mainFrameService"></param>
    /// <param name="icon"></param>
    /// <param name="title"></param>
    public static void AddFooterMenu<TPage>(this IMainFrameService mainFrameService, SymbolRegular icon, string title)
    {
        mainFrameService.AddFooterMenu(new NavigationViewItem(title, icon, typeof(TPage)));
    }

    /// <summary>
    /// 添加菜单
    /// </summary>
    /// <typeparam name="TPage"></typeparam>
    /// <param name="mainFrameService"></param>
    /// <param name="icon"></param>
    /// <param name="title"></param>
    /// <param name="builderAction"></param>
    public static void AddMenu<TPage>(this IMainFrameService mainFrameService, SymbolRegular icon, string title, Action<NavigationViewItemBuilder> builderAction)
    {
        var builder = NavigationViewItemBuilder.Create<TPage>(icon, title);
        builderAction.Invoke(builder);

        mainFrameService.AddMenu(builder.Build());
    }

    /// <summary>
    /// 添加页脚菜单
    /// </summary>
    /// <typeparam name="TPage"></typeparam>
    /// <param name="mainFrameService"></param>
    /// <param name="icon"></param>
    /// <param name="title"></param>
    /// <param name="builderAction"></param>
    public static void AddFooterMenu<TPage>(this IMainFrameService mainFrameService, SymbolRegular icon, string title, Action<NavigationViewItemBuilder> builderAction)
    {
        var builder = NavigationViewItemBuilder.Create<TPage>(icon, title);
        builderAction.Invoke(builder);

        mainFrameService.AddFooterMenu(builder.Build());
    }



    /// <summary>
    ///     刷新菜单
    /// </summary>
    /// <param name="mainFrameService"></param>
    /// <param name="menu"></param>
    /// <returns></returns>
    public static NavigationViewItem ReloadMenu(this IMainFrameService mainFrameService, NavigationViewItem menu)
    {
        if (mainFrameService.MenuItems.Contains(menu))
        {
            var menus = mainFrameService.MenuItems.ToArray();
            mainFrameService.MenuItems.Clear();
            menus.ForEach(mainFrameService.MenuItems.Add);
            return menu;
        }

        if (mainFrameService.FooterMenuItems.Contains(menu))
        {
            var menus = mainFrameService.FooterMenuItems.ToArray();
            mainFrameService.MenuItems.Clear();
            menus.ForEach(mainFrameService.MenuItems.Add);
            return menu;
        }

        return menu;
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


public class NavigationViewItemBuilder
{
    /// <summary>
    /// 构建方法
    /// </summary>
    /// <typeparam name="TPage"></typeparam>
    /// <param name="icon"></param>
    /// <param name="title"></param>
    /// <returns></returns>
    public static NavigationViewItemBuilder Create<TPage>(SymbolRegular icon, string title)
    {
        var item = new NavigationViewItem(title, icon, typeof(TPage));

        return new NavigationViewItemBuilder(item);
    }

   

    ///// <summary>
    ///// 设置缓存模式
    ///// </summary>
    ///// <param name="mode"></param>
    ///// <returns></returns>
    //public NavigationViewItemBuilder SetCacheMode(NavigationCacheMode mode)
    //{
    //    _self.NavigationCacheMode = mode;
    //    return this;
    //}




    /// <summary>
    /// 添加子菜单
    /// </summary>
    /// <typeparam name="TPage"></typeparam>
    /// <param name="icon"></param>
    /// <param name="title"></param>
    /// <returns></returns>
    public NavigationViewItemBuilder AddChild<TPage>(SymbolRegular icon, string title)
    {
        var item = new NavigationViewItem(title, icon, typeof(TPage));
        _self.MenuItems.Add(item);

        return new NavigationViewItemBuilder(item, this);
    }

    /// <summary>
    /// 返回父级构建器,如果是顶级则返回自己
    /// </summary>
    /// <returns></returns>
    public NavigationViewItemBuilder Parent() => _parent;

    /// <summary>
    /// 构建完成
    /// </summary>
    /// <returns></returns>
    public NavigationViewItem Build()
    {
        return _parent == this ? _self : _parent.Build();
    }



    private NavigationViewItemBuilder(NavigationViewItem item, NavigationViewItemBuilder? parent = null)
    {
        _self = item;
        _parent = parent ?? this;
    }

    private readonly NavigationViewItem _self;
    private readonly NavigationViewItemBuilder _parent;
}