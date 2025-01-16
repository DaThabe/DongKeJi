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
/// 核心模块
/// </summary>
public interface ICoreModule : IModule
{
    /// <summary>
    ///     主窗口信息
    /// </summary>
    MainFrameViewModel MainFrame { get; }

    /// <summary>
    /// 当前用户
    /// </summary>
    UserViewModel? CurrentUser { get; }
}


/// <summary>
/// 核心模块
/// </summary>
[Inject(ServiceLifetime.Singleton, typeof(ICoreModule))]
public partial class CoreModule(IApplication application) : ObservableViewModel, ICoreModule
{
    /// <summary>
    ///     主窗口
    /// </summary>
    [ObservableProperty] private MainFrameViewModel _mainFrame = new()
    {
        Title = application.Title
    };

    /// <summary>
    ///     当前用户
    /// </summary>
    [ObservableProperty] private UserViewModel? _currentUser;


    public static IModuleMetaInfo MetaInfo { get; } = new ModuleMetaInfo()
    {
        Id = Guid.NewGuid(),
        Name = "DongKeJi.Core",
        Version = new Version(0, 0, 1),
        Title = "核心",
        Developers = ["DaThabe"],
        Describe = """
                   ### 核心模块
                   - 程序框架
                   - 用户服务
                   - 配置服务
                   """,
        CreatedAt = new DateTime(2024, 8, 29),
        ReleaseDate = new DateTime(2025, 1, 2),
        Dependencies = typeof(CoreModule).Assembly.GetReferencedAssemblies()
    };

    public static void Configure(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            //导航服务
            services.AddSingleton<INavigationService, NavigationService>();
            //页面服务
            services.AddSingleton<IPageService, PageService>();
            //通知服务
            services.AddSingleton<ISnackbarService, SnackbarService>();
            //内部弹窗服务
            services.AddSingleton<IContentDialogService, ContentDialogService>();
            // 主题切换服务
            services.AddSingleton<IThemeService, ThemeService>();
            // 任务栏服务
            services.AddSingleton<ITaskBarService, TaskBarService>();


            //反射注入
            services.AddAutoInject<CoreModule>();
            //启动后业务
            services.AddHostedService<HostedService>();
            //数据库
            services.AddDbContext<CoreDbContext>();
            //AutoMapper
            services.AddAutoMapper(typeof(CoreMapperProfile));
        });
    }
}