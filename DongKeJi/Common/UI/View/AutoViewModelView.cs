using DongKeJi.Common.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DongKeJi.Common.UI.View;

/// <summary>
/// 自动VM View (导航进入自动加载ViewModel, 离开自动释放
/// </summary>
/// <param name="services"></param>
public abstract class AutoViewModelView(IServiceProvider services) : NavigationView(services)
{
    private readonly ILogger _logger = services.GetRequiredService<ILogger<AutoViewModelView>>();

    protected override async Task OnNavigatedToAsync(IServiceProvider services,
        CancellationToken cancellation = default)
    {
        await base.OnNavigatedToAsync(services, cancellation);


        var result = await this.LoadDataContextAsync(async x =>
        {
            _logger.LogTrace("正在加载视图模型: 视图类型:{type}", GetType().Name);
            var vm = await OnLoadViewModelAsync(services, x);
            _logger.LogTrace("视图模型加载成功, 视图类型:{view}, 模型类型:{vm}", GetType().Name, vm.GetType().Name);

            return vm;

        }, ExceptionCallback, cancellation);
        return;


        void ExceptionCallback(Exception ex)
        {
            _logger.LogError(ex, "视图模型加载失败: 视图类型:{type}", GetType().Name);
        }
    }

    protected override Task OnNavigatedFromAsync(IServiceProvider services, CancellationToken cancellation = default)
    {
        var dataContext = this.ReleaseDataContext();

        if (dataContext is not null)
        {
            _logger.LogTrace("视图模型已释放, 视图类型:{view}, 模型类型:{vm}", GetType().Name, dataContext.GetType().Name);
        }

        return base.OnNavigatedFromAsync(services, cancellation);
    }

    protected abstract ValueTask<IViewModel> OnLoadViewModelAsync(IServiceProvider services, CancellationToken cancellation = default);
}