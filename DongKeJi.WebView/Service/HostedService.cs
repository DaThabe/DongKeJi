using DongKeJi.Core.Service;
using DongKeJi.WebView.UI.View;
using Microsoft.Extensions.Hosting;
using Wpf.Ui.Controls;

namespace DongKeJi.WebView.Service;

internal class HostedService(IMainFrameService mainFrameService) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        //��Ӳ˵�
        mainFrameService.AddMenu<WebPage>(SymbolRegular.Globe20, "��ҳ");
        return Task.CompletedTask;
    }


    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}