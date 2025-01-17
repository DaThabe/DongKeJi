using DongKeJi.Core.Service;
using DongKeJi.Extensions;
using DongKeJi.Tool.Model;
using DongKeJi.Tool.UI.View;
using DongKeJi.Tool.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Wpf.Ui.Controls;

namespace DongKeJi.Tool.Service;

internal class HostedService(
    ILogger<HostedService> logger,
    IMainFrameService mainFrameService,
    IServiceProvider services,
    IToolPageService toolPageService
) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        //��ʼ�����й���
        toolPageService.ToolItems = GetAllToolItemView()
            .Select(x => new ToolItemViewModel(x.Icon, x.Title, x.PageType))
            .ToObservableCollection();

        //��Ӳ˵�
        mainFrameService.AddMenu<ToolPage>(SymbolRegular.Toolbox12, "����");

        return Task.CompletedTask;
    }


    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }



    private IEnumerable<IToolItem> GetAllToolItemView()
    {
        var moduleTools = services.GetServices<IModuleTool>();

        foreach (var i in moduleTools)
        {
            foreach (var j in i.ToolItems.Select(x => x))
            {
                yield return j;
            }
        }
    }

}