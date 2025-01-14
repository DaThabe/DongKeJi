using DongKeJi.Core;
using DongKeJi.Core.Service;
using DongKeJi.Core.UI.View;
using DongKeJi.Launcher.UI.View.Color;
using Microsoft.Extensions.Hosting;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace DongKeJi.Launcher.Service;

internal class HostedService(
    ICoreConfig coreConfig,
    IApplication application,
    IMainFrameService mainFrameService) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            application.Theme = await coreConfig.ApplicationTheme.GetAsync(cancellation: cancellationToken);
        }
        catch
        {
            application.Theme = ApplicationTheme.Light;
            await coreConfig.ApplicationTheme.SetAsync(application.Theme, cancellationToken);
        }

        mainFrameService.InsertFooterMenu<ColorView>(0, SymbolRegular.Color16, "颜色");

        var firstMenuItem = mainFrameService.MenuItems.FirstOrDefault();
        if (firstMenuItem is not null) firstMenuItem.IsActive = true;

        mainFrameService.Show();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}