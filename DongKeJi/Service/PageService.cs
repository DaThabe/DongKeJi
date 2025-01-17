using System.Windows;
using Microsoft.Extensions.Logging;
using Wpf.Ui;

namespace DongKeJi.Service;

/// <summary>
///     ҳ�����ʵ��
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
            logger.LogError("��ȡҳ��ʧ��, ��Ϊ������Ч��Wpfҳ��");
            return null;
        }

        var page =  services.GetService(pageType) as FrameworkElement;
        if (page is null)
        {
            logger.LogError("��ȡҳ��ʧ��, δע�������: {type}", pageType.Name);
        }

        return page;
    }
}