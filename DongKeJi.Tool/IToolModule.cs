using DongKeJi.Inject;
using DongKeJi.Module;
using DongKeJi.Tool.Model;
using DongKeJi.Tool.Service;
using DongKeJi.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DongKeJi.Tool;


/// <summary>
/// 办公模块
/// </summary>
public interface IToolModule : IModule;

/// <summary>
/// 工具模块
/// </summary>
[Inject(ServiceLifetime.Singleton, typeof(IToolModule))]
public class ToolModule : ObservableViewModel, IToolModule
{
    public static IModuleInfo Info { get; }= new ModuleInfo
    {
        Id = Guid.NewGuid(),
        Name = "DongKeJi.Tool",
        Version = new Version(0, 0, 1),
        Title = "工具",
        Developers = ["DaThabe"],
        Describe = """
                   ### 工具模块
                   - 一些小工具
                   """,
        CreatedAt = new DateTime(2025, 1, 17),
        ReleaseDate = new DateTime(2025, 1, 17),
        Dependencies = typeof(ToolModule).Assembly.GetReferencedAssemblies()
    };

    public static void Configure(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            //注册工具
            services.AddToolItems<ToolModule>();

            //反射注入
            services.AddAutoInject<ToolModule>();
            //启动后业务
            services.AddHostedService<HostedService>();
            
            //数据库
            //services.AddDbContext<WorkDbContext>();
            //AutoMapper
            //services.AddAutoMapper(typeof(WorkMapperProfile));
        });
    }
}