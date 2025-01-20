using DongKeJi.Core;
using DongKeJi.Core.Service;
using DongKeJi.Core.UI.View.Frame;
using DongKeJi.Launcher.UI.View.Color;
using DongKeJi.Work.UI.View;
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

#if DEBUG
        mainFrameService.AddFooterMenu<ColorView>(SymbolRegular.Color16, "颜色");
#endif
        mainFrameService.Show();
        await mainFrameService.NavigationAsync(typeof(CustomerPage));
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}