using System.Windows;
using Microsoft.Extensions.Logging;
using Wpf.Ui;

namespace DongKeJi.Service;

/// <summary>
///     页面服务实现
/// </summary>
public class PageService(IServiceProvider services, ILogger<PageService> logger) : IPageService
{
    public T? GetPage<T>() where T : class
    {
        return GetPage(typeof(T)) as T;
    }

    public FrameworkElement? GetPage(Type pageType)
    {
        if (!typeof(FrameworkElement).IsAssignableFrom(pageType))
        {
            logger.LogError("获取页面失败, 因为不是有效的Wpf页面");
            return null;
        }

        var page =  services.GetService(pageType) as FrameworkElement;
        if (page is null)
        {
            logger.LogError("获取页面失败, 未注册的类型: {type}", pageType.Name);
        }

        return page;
    }
}