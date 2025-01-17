using DongKeJi.Config;
using DongKeJi.Core.Model;
using DongKeJi.Inject;
using Microsoft.Extensions.DependencyInjection;
using Wpf.Ui.Appearance;

namespace DongKeJi.Core;


/// <summary>
/// 核心模块配置
/// </summary>
public interface ICoreConfig : IConfigService
{
    /// <summary>
    /// 记住的用户Id
    /// </summary>
    IConfigItem<Guid> RememberUserId { get; }

    /// <summary>
    /// 程序主题
    /// </summary>
    IConfigItem<ApplicationTheme> ApplicationTheme { get; }
}


[Inject(ServiceLifetime.Singleton, typeof(ICoreConfig))]
internal class CoreConfigService(CoreDbContext dbContext) : ICoreConfig
{
    public IConfigItem<Guid> RememberUserId { get; } =
        new ConfigItem<CoreDbContext, Guid>(nameof(RememberUserId), dbContext);

    public IConfigItem<ApplicationTheme> ApplicationTheme { get; } =
        new ConfigItem<CoreDbContext, ApplicationTheme>(nameof(ApplicationTheme), dbContext);
}