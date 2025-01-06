using DongKeJi.Common;
using DongKeJi.Common.Exceptions;
using DongKeJi.Common.Inject;
using DongKeJi.Common.Module;
using DongKeJi.Model;
using DongKeJi.Service;
using DongKeJi.UI.View.Setting;
using DongKeJi.UI.View.User;
using DongKeJi.ViewModel;
using DongKeJi.ViewModel.User;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace DongKeJi;

public class CoreModule : IModule
{
    public string Title => "懂科技核心模块";
    public string Describe => "提供核心功能";


    public void Configure(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddAutoInject<CoreModule>();
            services.AddHostedService<DongKeJiHostedService>();

            //导航服务
            services.AddSingleton<INavigationService, NavigationService>();
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

file class DongKeJiHostedService(
    ILogger<DongKeJiHostedService> logger,
    IMainFrameService mainFrameService,
    IApplicationContext applicationContext,
    IUserRepository userRepository) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
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
            try
            {
                var users = await userRepository.GetAllAsync(cancellation: cancellationToken);
                var currentUser = users.FirstOrDefault();

                if (currentUser is null || currentUser.IsEmpty())
                {
                    throw new RepositoryException("用户获取失败, 没有用户数据");
                }

                applicationContext.LoginUser = currentUser;
            }
            catch (RepositoryException)
            {
                var user = new UserViewModel { Name = "Admin" };
                await userRepository.AddAsync(user, cancellationToken);
                applicationContext.LoginUser = user;
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "用户初始化失败");
        }
    }
}