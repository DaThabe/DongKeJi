using DongKeJi.Config;
using DongKeJi.Core.Model;
using DongKeJi.Inject;
using Microsoft.Extensions.DependencyInjection;

namespace DongKeJi.Core.Service;


/// <summary>
/// 核心配置
/// </summary>
public interface ICoreConfigService : IConfigService
{
    /// <summary>
    /// 记住的用户Id
    /// </summary>
    IConfigItem<Guid> RememberUserId { get; }
}


[Inject(ServiceLifetime.Singleton, typeof(ICoreConfigService))]
internal class CoreConfigService(CoreDbContext dbContext) : ICoreConfigService
{
    public IConfigItem<Guid> RememberUserId { get; } =
        new ConfigItem<CoreDbContext, Guid>(nameof(RememberUserId), dbContext);
}