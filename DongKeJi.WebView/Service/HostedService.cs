using DongKeJi.Core.Service;
using DongKeJi.WebView.UI.View;
using Microsoft.Extensions.Hosting;
using Wpf.Ui.Controls;

namespace DongKeJi.WebView.Service;

internal class HostedService(IMainFrameService mainFrameService) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        //Ìí¼Ó²Ëµ¥
        mainFrameService.AddMenu<WebPage>(SymbolRegular.Globe20, "ÍøÒ³");
        return Task.CompletedTask;
    }


    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}