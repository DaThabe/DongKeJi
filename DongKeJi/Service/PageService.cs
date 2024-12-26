using Microsoft.Extensions.Logging;
using System.Windows;
using DongKeJi.Common.Inject;
using Microsoft.Extensions.DependencyInjection;
using Wpf.Ui;

namespace DongKeJi.Service;


/// <summary>
/// 页面服务实现
/// </summary>
[Inject(ServiceLifetime.Singleton, typeof(IPageService))]
internal class PageService(IServiceProvider services, ILogger<PageService> logger) : IPageService
{
    public T? GetPage<T>() where T : class
    {
        return GetPage(typeof(T)) as T;
    }

    public FrameworkElement? GetPage(Type pageType)
    {
        if (!typeof(FrameworkElement).IsAssignableFrom(pageType))
        {
            var ex = new InvalidOperationException("The page should be a WPF control.");
            logger.LogError(ex, "获取页面失败, 因为不是有效的Wpf页面");
            return null;
        }

        return services.GetService(pageType) as FrameworkElement;
    }
}
