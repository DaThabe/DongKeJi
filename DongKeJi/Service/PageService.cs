using Microsoft.Extensions.Logging;
using System.Windows;
using DongKeJi.Common.Inject;
using Microsoft.Extensions.DependencyInjection;
using Wpf.Ui;

namespace DongKeJi.Service;


/// <summary>
/// ҳ�����ʵ��
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
            logger.LogError(ex, "��ȡҳ��ʧ��, ��Ϊ������Ч��Wpfҳ��");
            return null;
        }

        return services.GetService(pageType) as FrameworkElement;
    }
}
