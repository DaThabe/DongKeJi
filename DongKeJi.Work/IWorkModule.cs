using CommunityToolkit.Mvvm.ComponentModel;
using DongKeJi.Core;
using DongKeJi.Core.Service;
using DongKeJi.Inject;
using DongKeJi.Module;
using DongKeJi.ViewModel;
using DongKeJi.Work.Model;
using DongKeJi.Work.Service;
using DongKeJi.Work.ViewModel.Staff;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DongKeJi.Work;


/// <summary>
/// �칫ģ��
/// </summary>
public interface IWorkModule : IModule
{
    /// <summary>
    /// ��ǰԱ��
    /// </summary>
    StaffViewModel? CurrentStaff { get; }
}

/// <summary>
/// �칫ģ��
/// </summary>
[Inject(ServiceLifetime.Singleton, typeof(IWorkModule))]
public partial class WorkModule : ObservableViewModel, IWorkModule
{
    [ObservableProperty]
    private StaffViewModel? _currentStaff;


    public static IModuleMetaInfo MetaInfo { get; }= new ModuleMetaInfo()
    {
        Id = Guid.NewGuid(),
        Name = "DongKeJi.Work",
        Version = new Version(0, 0, 1),
        Title = "�칫",
        Developers = ["DaThabe"],
        Describe = """
                   ### �칫ģ��
                   - ������ϸ��¼
                   """,
        CreatedAt = new DateTime(2024, 9, 17),
        ReleaseDate = new DateTime(2025, 1, 12),
        Dependencies = typeof(WorkModule).Assembly.GetReferencedAssemblies()
    };

    public static void Configure(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            //����ע��
            services.AddAutoInject<WorkModule>();
            //������ҵ��
            services.AddHostedService<HostedService>();
            //���ݿ�
            services.AddDbContext<WorkDbContext>();
            //AutoMapper
            services.AddAutoMapper(typeof(WorkMapperProfile));
        });
    }
}