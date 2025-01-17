using DongKeJi.Core;
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