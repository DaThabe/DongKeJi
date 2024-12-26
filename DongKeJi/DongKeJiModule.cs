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
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace DongKeJi;


public class DongKeJiModule : IModule
{
    public string Title => "懂科技核心模块";
    public string Describe => "提供核心功能";


    public void Configure(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddAutoInject<DongKeJiModule>();
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
            services.AddDbContext<DongKeJiDbContext>();
            //AutoMapper
            services.AddAutoMapper(typeof(DongKeJiMapperProfile));
        });
    }
}


file class DongKeJiHostedService(
    IMainFrameService mainFrameService,
    IApplicationContext applicationContext,
    IUserService userService) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        mainFrameService.AddFooterMenu<UserDashboardView>(SymbolRegular.People20, "用户");
        mainFrameService.AddFooterMenu<SettingDashboardView>(SymbolRegular.Settings28, "设置");


        var user = await userService.FindByNameAsync("DaThabe", cancellationToken);
        
        if (user is null)
        {
            user = new UserViewModel { Name = "DaThabe" };
            await userService.CreateAsync(user, cancellationToken);
        }

        applicationContext.User = user;
        applicationContext.User.IsLogged = true;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
