using System.Collections.ObjectModel;
using DongKeJi.Inject;
using DongKeJi.Tool.ViewModel;
using Microsoft.Extensions.DependencyInjection;

namespace DongKeJi.Tool.Service;

/// <summary>
/// 工具服务
/// </summary>
public interface IToolPageService
{
    /// <summary>
    /// 所有工具元素
    /// </summary>
    ObservableCollection<ToolItemViewModel> ToolItems { get; set; }
}


[Inject(ServiceLifetime.Singleton, typeof(IToolPageService))]
internal class ToolPageService(ToolPageViewModel toolPageViewModel) : IToolPageService
{
    public ObservableCollection<ToolItemViewModel> ToolItems
    {
        get => toolPageViewModel.ToolItemCollection;
        set => toolPageViewModel.ToolItemCollection = value;
    }
}