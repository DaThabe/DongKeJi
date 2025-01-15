using DongKeJi.Core.Model;
using DongKeJi.Core.UI.View.Module;
using DongKeJi.Core.UI.View.Setting;
using DongKeJi.Core.UI.View.User;
using DongKeJi.Core.ViewModel.User;
using DongKeJi.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Wpf.Ui.Controls;

namespace DongKeJi.Core.Service;

internal class HostedService(
    ILogger<HostedService> logger,
    IMainFrameService mainFrameService,
    ICoreDatabase database,
    CoreDbContext dbContext,
    IUserService userService) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            await dbContext.UnitOfWorkAsync(async t =>
            {
                await dbContext.Database.MigrateAsync(cancellationToken: cancellationToken);

            }, cancellationToken);

        }
        catch (Exception e)
        {
            logger.LogError(e, "�������ݿ�Ǩ��ʧ��, �ѻع�");
        }


        mainFrameService.ShowWindow();

        mainFrameService.AddFooterMenu<ModuleDashboardView>(SymbolRegular.DeveloperBoard16, "ģ��");
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
            var userId = await userService.GetRememberUserIdAsync(cancellationToken);
            var user = await userService.GetByIdAsync(userId, cancellationToken);

            await userService.LoginAsync(user, true, cancellation: cancellationToken);
            database.AutoUpdate(user);
        }
        catch
        {
            try
            {
                var users = await userService.GetAllAsync(cancellation: cancellationToken);
                var currentUser = users.FirstOrDefault();

                if (currentUser is null)
                {
                    throw new ArgumentNullException(nameof(currentUser), "�û���ȡʧ��, û���û�����");
                }

                await userService.LoginAsync(currentUser, cancellation: cancellationToken);
                database.AutoUpdate(currentUser);
            }
            catch (ArgumentNullException)
            {
                var user = new UserViewModel { Name = "Admin" };
                await userService.AddAsync(user, cancellationToken);

                await userService.LoginAsync(user, true, cancellation: cancellationToken);
                database.AutoUpdate(user);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "�û����ݳ�ʼ��ʧ��!");
            }
        }
    }
}