using DongKeJi.Inject;
using DongKeJi.Module;
using DongKeJi.Tool.Model;
using DongKeJi.Tool.Service;
using DongKeJi.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DongKeJi.Tool;


/// <summary>
/// �칫ģ��
/// </summary>
public interface IToolModule : IModule;

/// <summary>
/// ����ģ��
/// </summary>
[Inject(ServiceLifetime.Singleton, typeof(IToolModule))]
public class ToolModule : ObservableViewModel, IToolModule
{
    public static IModuleInfo Info { get; }= new ModuleInfo
    {
        Id = Guid.NewGuid(),
        Name = "DongKeJi.Tool",
        Version = new Version(0, 0, 1),
        Title = "����",
        Developers = ["DaThabe"],
        Describe = """
                   ### ����ģ��
                   - һЩС����
                   """,
        CreatedAt = new DateTime(2025, 1, 17),
        ReleaseDate = new DateTime(2025, 1, 17),
        Dependencies = typeof(ToolModule).Assembly.GetReferencedAssemblies()
    };

    public static void Configure(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            //ע�Ṥ��
            services.AddToolItems<ToolModule>();

            //����ע��
            services.AddAutoInject<ToolModule>();
            //������ҵ��
            services.AddHostedService<HostedService>();
            
            //���ݿ�
            //services.AddDbContext<WorkDbContext>();
            //AutoMapper
            //services.AddAutoMapper(typeof(WorkMapperProfile));
        });
    }
}