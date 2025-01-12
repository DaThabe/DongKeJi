using DongKeJi.Inject;
using Microsoft.Extensions.DependencyInjection;
using Wpf.Ui.Appearance;

namespace DongKeJi.Core.Service;

/// <summary>
///     程序服务
/// </summary>
public interface IApplicationService
{
    /// <summary>
    ///     标题
    /// </summary>
    string Title { get; set; }

    /// <summary>
    ///     版本
    /// </summary>
    Version Version { get; set; }

    /// <summary>
    ///     主题
    /// </summary>
    ApplicationTheme Theme { get; set; }
}

[Inject(ServiceLifetime.Singleton, typeof(IApplicationService))]
public class ApplicationService(ICoreContext coreContext) : IApplicationService
{
    public string Title
    {
        get => coreContext.Application.Title;
        set => coreContext.Application.Title = value;
    }

    public Version Version
    {
        get => coreContext.Application.Version;
        set => coreContext.Application.Version = value;
    }

    public ApplicationTheme Theme
    {
        get => coreContext.Application.Theme;
        set => coreContext.Application.Theme = value;
    }
}