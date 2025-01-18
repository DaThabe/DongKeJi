using DongKeJi.Inject;
using DongKeJi.Launcher.Service;
using DongKeJi.Module;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DongKeJi.Launcher;


/// <summary>
/// 启动器模块
/// </summary>
public interface ILauncherModule : IModule;


[Inject(ServiceLifetime.Singleton)]
internal class LauncherModule : ILauncherModule
{
    public static IModuleInfo Info { get; } = new ModuleInfo()
    {
        Id = Guid.NewGuid(),
        Name = "DongKeJi.Launcher",
        Version = new Version(0, 0, 1),
        Title = "启动器",
        Developers = ["DaThabe"],
        Describe = """
                   ### 启动器模块
                   - 检测更新
                   - 加载功能
                   """,
        CreatedAt = new DateTime(2024, 10, 3),
        ReleaseDate = new DateTime(2025, 1, 2),
        Dependencies = typeof(LauncherModule).Assembly.GetReferencedAssemblies()
    };

    public static void Configure(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            //反射注入
            services.AddAutoInject<LauncherModule>();
            //启动后业务
            services.AddHostedService<HostedService>();

            //数据库
            //services.AddDbContext<CoreDbContext>();
            //AutoMapper
            //services.AddAutoMapper(typeof(CoreMapperProfile));
        });
    }
}


///// <summary>
///// 模块迁移服务
///// </summary>
//public interface IModuleMigrationService
//{
//    /// <summary>
//    /// 执行迁移, 从指定版本
//    /// </summary>
//    /// <param name="version"></param>
//    ValueTask ExecuteAsync(Version version);
//}


//public class ModuleMigrationService : IModuleMigrationService
//{
//    public ValueTask ExecuteAsync(Version version)
//    {
//        _migrationActions.TryGetValue(version, out var value);
//    }

//    /// <summary>
//    /// 设置迁移
//    /// </summary>
//    /// <param name="version"></param>
//    /// <param name="action"></param>
//    protected void SetMigration(Version version, IMigrationAction action)
//    {
//        _migrationActions[version] = action;
//    }


//    /// <summary>
//    /// 所有迁移动作
//    /// </summary>
//    private readonly Dictionary<Version, IMigrationAction> _migrationActions = [];
//}

///// <summary>
///// 迁移动作
///// </summary>
//public interface IMigrationAction
//{
//    /// <summary>
//    /// 执行该迁移的客户端版本
//    /// </summary>
//    Version version { get; }

//    /// <summary>
//    /// 执行迁移
//    /// </summary>
//    /// <returns></returns>
//     ValueTask ExecuteAsync();

//    /// <summary>
//    /// 迁移失败的回滚过程
//    /// </summary>
//    /// <returns></returns>
//     ValueTask RollbackAsync();
//}

//public class V001MigrationAction : IMigrationAction
//{
//    public ValueTask ExecuteAsync()
//    {
//        File.Copy(@"", "");

//        return ValueTask.CompletedTask;
//    }

//    public ValueTask RollbackAsync()
//    {
//        File.Copy(@"", "");

//        return ValueTask.CompletedTask;
//    }
//}