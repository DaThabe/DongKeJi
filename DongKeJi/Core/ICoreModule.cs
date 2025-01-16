using CommunityToolkit.Mvvm.ComponentModel;
using DongKeJi.Core.Model;
using DongKeJi.Core.Service;
using DongKeJi.Core.ViewModel.Frame;
using DongKeJi.Core.ViewModel.User;
using DongKeJi.Inject;
using DongKeJi.Module;
using DongKeJi.Service;
using DongKeJi.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wpf.Ui;
using UserViewModel = DongKeJi.Core.ViewModel.User.UserViewModel;

namespace DongKeJi.Core;


/// <summary>
/// ����ģ��
/// </summary>
public interface ICoreModule : IModule
{
    /// <summary>
    ///     ��������Ϣ
    /// </summary>
    MainFrameViewModel MainFrame { get; }

    /// <summary>
    /// ��ǰ�û�
    /// </summary>
    UserViewModel? CurrentUser { get; }
}


/// <summary>
/// ����ģ��
/// </summary>
[Inject(ServiceLifetime.Singleton, typeof(ICoreModule))]
public partial class CoreModule(IApplication application) : ObservableViewModel, ICoreModule
{
    /// <summary>
    ///     ������
    /// </summary>
    [ObservableProperty] private MainFrameViewModel _mainFrame = new()
    {
        Title = application.Title
    };

    /// <summary>
    ///     ��ǰ�û�
    /// </summary>
    [ObservableProperty] private UserViewModel? _currentUser;


    public static IModuleMetaInfo MetaInfo { get; } = new ModuleMetaInfo()
    {
        Id = Guid.NewGuid(),
        Name = "DongKeJi.Core",
        Version = new Version(0, 0, 1),
        Title = "����",
        Developers = ["DaThabe"],
        Describe = """
                   ### ����ģ��
                   - ������
                   - �û�����
                   - ���÷���
                   """,
        CreatedAt = new DateTime(2024, 8, 29),
        ReleaseDate = new DateTime(2025, 1, 2),
        Dependencies = typeof(CoreModule).Assembly.GetReferencedAssemblies()
    };

    public static void Configure(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            //��������
            services.AddSingleton<INavigationService, NavigationService>();
            //ҳ�����
            services.AddSingleton<IPageService, PageService>();
            //֪ͨ����
            services.AddSingleton<ISnackbarService, SnackbarService>();
            //�ڲ���������
            services.AddSingleton<IContentDialogService, ContentDialogService>();
            // �����л�����
            services.AddSingleton<IThemeService, ThemeService>();
            // ����������
            services.AddSingleton<ITaskBarService, TaskBarService>();


            //����ע��
            services.AddAutoInject<CoreModule>();
            //������ҵ��
            services.AddHostedService<HostedService>();
            //���ݿ�
            services.AddDbContext<CoreDbContext>();
            //AutoMapper
            services.AddAutoMapper(typeof(CoreMapperProfile));
        });
    }
}