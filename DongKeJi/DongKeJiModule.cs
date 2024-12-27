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

public class DongKeJiModule : IModule
{
    public string Title => "���Ƽ�����ģ��";
    public string Describe => "�ṩ���Ĺ���";


    public void Configure(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddAutoInject<DongKeJiModule>();
            services.AddHostedService<DongKeJiHostedService>();

            //��������
            services.AddSingleton<INavigationService, NavigationService>();
            //֪ͨ����
            services.AddSingleton<ISnackbarService, SnackbarService>();
            //�ڲ���������
            services.AddSingleton<IContentDialogService, ContentDialogService>();
            // �����л�����
            services.AddSingleton<IThemeService, ThemeService>();
            // ����������
            services.AddSingleton<ITaskBarService, TaskBarService>();


            //���ݿ�
            services.AddDbContext<DongKeJiDbContext>();
            //AutoMapper
            services.AddAutoMapper(typeof(DongKeJiMapperProfile));
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
        mainFrameService.AddFooterMenu<UserDashboardView>(SymbolRegular.People20, "�û�");
        mainFrameService.AddFooterMenu<SettingDashboardView>(SymbolRegular.Settings28, "����");

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
            UserViewModel user;

            try
            {
                user = await userRepository.FindByNameAsync("DaThabe", cancellationToken);
            }
            catch (RepositoryException ex)
            {
                var msg = ex.Message;
                var str = ex.ToString();

                user = new UserViewModel { Name = "DaThabe" };
                await userRepository.AddAsync(user, cancellationToken);
            }

            applicationContext.User = user;
            applicationContext.User.IsLogged = true;
        }
        catch (Exception e)
        {
            logger.LogError(e, "�û���ʼ��ʧ��");
        }
    }
}