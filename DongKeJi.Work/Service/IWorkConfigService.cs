using DongKeJi.Config;
using DongKeJi.Core.Service;
using DongKeJi.Inject;
using DongKeJi.Work.Model;
using Microsoft.Extensions.DependencyInjection;

namespace DongKeJi.Work.Service;

public interface IWorkConfigService : IConfigService
{
    /// <summary>
    /// 主员工和用户的绑定关系 用户-员工
    /// </summary>
    IConfigItem<Dictionary<Guid, Guid>> PrimaryStaff { get; }
}


[Inject(ServiceLifetime.Singleton, typeof(IWorkConfigService))]
internal class CoreConfigService(WorkDbContext dbContext) : IWorkConfigService
{
    public IConfigItem<Dictionary<Guid, Guid>> PrimaryStaff { get; } =
        new ConfigItem<WorkDbContext, Dictionary<Guid, Guid>>(nameof(PrimaryStaff), dbContext);
}