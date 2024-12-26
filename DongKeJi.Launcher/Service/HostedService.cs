using DongKeJi.Launcher.Model;
using DongKeJi.UI.View;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Wpf.Ui;

namespace DongKeJi.Launcher.Service;


internal class HostedService(
    ILoggerFactory loggerFactory,
    ISnackbarService snackbarService,
    MainFrame mainFrame) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        //loggerFactory.AddProvider(new SnakebarLoggerProvider(LogLevel.Trace, snackbarService));
        mainFrame.Show();

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}