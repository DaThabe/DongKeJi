using DongKeJi.UI.View;
using Microsoft.Extensions.Hosting;
using MainFrame = DongKeJi.Core.UI.View.MainFrame;

namespace DongKeJi.Launcher.Service;

internal class HostedService(
    MainFrame mainFrame) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        mainFrame.Show();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}