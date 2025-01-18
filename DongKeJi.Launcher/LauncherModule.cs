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


///// <summary>
///// ģ��Ǩ�Ʒ���
///// </summary>
//public interface IModuleMigrationService
//{
//    /// <summary>
//    /// ִ��Ǩ��, ��ָ���汾
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
//    /// ����Ǩ��
//    /// </summary>
//    /// <param name="version"></param>
//    /// <param name="action"></param>
//    protected void SetMigration(Version version, IMigrationAction action)
//    {
//        _migrationActions[version] = action;
//    }


//    /// <summary>
//    /// ����Ǩ�ƶ���
//    /// </summary>
//    private readonly Dictionary<Version, IMigrationAction> _migrationActions = [];
//}

///// <summary>
///// Ǩ�ƶ���
///// </summary>
//public interface IMigrationAction
//{
//    /// <summary>
//    /// ִ�и�Ǩ�ƵĿͻ��˰汾
//    /// </summary>
//    Version version { get; }

//    /// <summary>
//    /// ִ��Ǩ��
//    /// </summary>
//    /// <returns></returns>
//     ValueTask ExecuteAsync();

//    /// <summary>
//    /// Ǩ��ʧ�ܵĻع�����
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