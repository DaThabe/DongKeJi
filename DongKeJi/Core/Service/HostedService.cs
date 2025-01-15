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
            logger.LogError(e, "核心数据库迁移失败, 已回滚");
        }


        mainFrameService.ShowWindow();

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
                    throw new ArgumentNullException(nameof(currentUser), "用户获取失败, 没有用户数据");
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
                logger.LogError(ex, "用户数据初始化失败!");
            }
        }
    }
}