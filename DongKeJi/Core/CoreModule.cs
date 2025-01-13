using System.Collections;
using System.Resources;
using System.Windows;
using System.Windows.Controls;
using DongKeJi.Core.Model;
using DongKeJi.Core.Service;
using DongKeJi.Core.UI.View;
using DongKeJi.Core.UI.View.Module;
using DongKeJi.Core.UI.View.Setting;
using DongKeJi.Core.UI.View.User;
using DongKeJi.Core.ViewModel.User;
using DongKeJi.Exceptions;
using DongKeJi.Inject;
using DongKeJi.Module;
using DongKeJi.Service;
using DongKeJi.UI.View;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace DongKeJi.Core;


/// <summary>
/// 核心模块
/// </summary>
public class CoreModule : IModule
{
    private static readonly ModuleMetaInfo ModuleMetaInfo = new()
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
        Dependencies = []
    };

    public IModuleMetaInfo MetaInfo => ModuleMetaInfo;

    public void Configure(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddAutoInject<CoreModule>();
            services.AddHostedService<HostedService>();

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


            //数据库
            services.AddDbContext<CoreDbContext>();
            //AutoMapper
            services.AddAutoMapper(typeof(CoreMapperProfile));
        });
    }
}

file class HostedService(
    ILogger<HostedService> logger,
    IApplication application,
    IMainFrameService mainFrameService,
    ICoreDbService dbService,
    ICoreContext coreContext,
    IUserService userService) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        //// 动态加载样式
        //var resourceDictionary = new ResourceDictionary
        //{
        //    Source = new Uri("pack://application:,,,/DongKeJi;component/UI/Themes/Generic.xaml")
        //};
        //application.Resources.MergedDictionaries.Add(resourceDictionary);

        mainFrameService.Show();
        mainFrameService.AddFooterMenu<ModuleDashboardView>(SymbolRegular.DeveloperBoard16, "模块");
        mainFrameService.AddFooterMenu<UserDashboardView>(SymbolRegular.People20, "用户");
        mainFrameService.AddFooterMenu<SettingDashboardView>(SymbolRegular.Settings28, "设置");

        await InitUser(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private async ValueTask InitUser(CancellationToken cancellationToken)
    {
        try
        {
            var userId = await userService.GetRememberUserIdAsync(cancellationToken);
            var user = await userService.FindByIdAsync(userId, cancellationToken);

            await userService.LoginAsync(user, true, cancellation: cancellationToken);
            dbService.AutoUpdate(user);
        }
        catch
        {
            try
            {
                var users = await userService.GetAllAsync(cancellation: cancellationToken);
                var currentUser = users.FirstOrDefault();

                if (currentUser is null)
                {
                    throw new ArgumentNullException(nameof(currentUser), "用户获取失败, 没有用户数据");
                }

                await userService.LoginAsync(currentUser, cancellation: cancellationToken);
                dbService.AutoUpdate(currentUser);
            }
            catch (ArgumentNullException)
            {
                var user = new UserViewModel { Name = "Admin" };
                await userService.AddAsync(user, cancellationToken);

                await userService.LoginAsync(user, true, cancellation: cancellationToken);
                dbService.AutoUpdate(user);
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "用户数据初始化失败!");
            }
        }
    }
}