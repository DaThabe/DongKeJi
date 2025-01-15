using DongKeJi.Config;
using DongKeJi.Inject;
using DongKeJi.Work.Model;
using Microsoft.Extensions.DependencyInjection;

namespace DongKeJi.Work;


/// <summary>
/// 办公模块配置
/// </summary>
public interface IWorkConfig : IConfigService
{
    /// <summary>
    /// 主员工和用户的绑定关系 用户-员工
    /// </summary>
    IConfigItem<Dictionary<Guid, Guid>> PrimaryStaff { get; }

    /// <summary>
    /// 基础工资
    /// </summary>
    IConfigItem<double> BasicSalary { get; }

    /// <summary>
    /// 提成百分比
    /// </summary>
    IConfigItem<double> CommissionPercentage { get; }
}


[Inject(ServiceLifetime.Singleton, typeof(IWorkConfig))]
internal class CoreConfigService(WorkDbContext dbContext) : IWorkConfig
{
    public IConfigItem<Dictionary<Guid, Guid>> PrimaryStaff { get; } =
        new ConfigItem<WorkDbContext, Dictionary<Guid, Guid>>(nameof(PrimaryStaff), dbContext);

    public IConfigItem<double> BasicSalary { get; } =
        new ConfigItem<WorkDbContext, double>(nameof(BasicSalary), dbContext);

    public IConfigItem<double> CommissionPercentage { get; } =
        new ConfigItem<WorkDbContext, double>(nameof(CommissionPercentage), dbContext);
}