using DongKeJi.Core;
using DongKeJi.Inject;
using DongKeJi.Launcher.Service;
using DongKeJi.Module;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DongKeJi.Launcher;


/// <summary>
/// ������ģ��
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
        Title = "������",
        Developers = ["DaThabe"],
        Describe = """
                   ### ������ģ��
                   - ������
                   - ���ع���
                   """,
        CreatedAt = new DateTime(2024, 10, 3),
        ReleaseDate = new DateTime(2025, 1, 2),
        Dependencies = typeof(LauncherModule).Assembly.GetReferencedAssemblies()
    };

    public static void Configure(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            //����ע��
            services.AddAutoInject<LauncherModule>();
            //������ҵ��
            services.AddHostedService<HostedService>();

            //���ݿ�
            //services.AddDbContext<CoreDbContext>();
            //AutoMapper
            //services.AddAutoMapper(typeof(CoreMapperProfile));
        });
    }
}