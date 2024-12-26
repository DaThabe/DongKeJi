using DongKeJi.Common.Inject;
using DongKeJi.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Wpf.Ui.Appearance;

namespace DongKeJi.Service;

/// <summary>
/// 程序服务
/// </summary>
public interface IApplicationService
{
    /// <summary>
    /// 标题
    /// </summary>
    string Title { get; set; }

    /// <summary>
    /// 版本
    /// </summary>
    Version Version { get; set; }

    /// <summary>
    /// 主题
    /// </summary>
    ApplicationTheme Theme { get; set; }
}


[Inject(ServiceLifetime.Singleton, typeof(IApplicationService))]
public class ApplicationService(IApplicationContext applicationContext) : IApplicationService
{
    public string Title
    {
        get => applicationContext.Application.Title;
        set => applicationContext.Application.Title = value;
    }

    public Version Version
    {
        get => applicationContext.Application.Version;
        set => applicationContext.Application.Version = value;
    }

    public ApplicationTheme Theme
    {
        get => applicationContext.Application.Theme;
        set => applicationContext.Application.Theme = value;
    }
}