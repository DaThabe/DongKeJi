using DongKeJi.Core;
using DongKeJi.Core.UI.View;
using DongKeJi.Inject;
using DongKeJi.Module;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DongKeJi.Launcher;

public class LauncherModule : IModule
{
    private static readonly ModuleMetaInfo ModuleMetaInfo = new()
    {
        Id = Guid.NewGuid(),
        Version = new Version(0, 0, 0),
        Title = "������",
        Developers = ["DaThabe"],
        Describe = """
                   ### ������ģ��
                   - ������
                   - ���ع���
                   """,
        CreatedAt = new DateTime(2024, 10, 3),
        ReleaseDate = new DateTime(2025, 1, 2),
        Dependencies =
        [
            typeof(CoreModule).Assembly.GetName(),
            //typeof(WorkModule).Assembly.GetName()
        ]
    };

    public IModuleMetaInfo MetaInfo => ModuleMetaInfo;

    public void Configure(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddAutoInject<LauncherModule>();
            services.AddHostedService<HostedService>();
        });
    }
}
internal class HostedService(
    MainFrame mainFrame) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        mainFrame.Show();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}