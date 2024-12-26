using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace DongKeJi.Common.UI.View;


/// <summary>
/// 
/// </summary>
public abstract class NavigationView(IServiceProvider services) : ContentControl, INavigationAware, IInfrastructure
{
    /// <summary>
    /// 异步导航取消
    /// </summary>
    private CancellationTokenSource? _cancellationTokenSource;

    /// <summary>
    /// 
    /// </summary>
    public IServiceProvider ServiceProvider => services;

    /// <summary>
    /// 日志
    /// </summary>
    private readonly ILogger<NavigationView> _logger = services.GetRequiredService<ILogger<NavigationView>>();

    /// <summary>
    /// 底部弹窗
    /// </summary>
    private readonly ISnackbarService _snackbarService = services.GetRequiredService<ISnackbarService>();


    /// <summary>
    /// 是否进入控件了
    /// </summary>
    public new bool IsLoaded { get; private set; }


    /// <summary>
    /// 导航进入此控件
    /// </summary>
    public async void OnNavigatedTo()
    {
        try
        {
            if (_cancellationTokenSource is not null)
            {
                await _cancellationTokenSource.CancelAsync();
            }

            _cancellationTokenSource = new CancellationTokenSource();

            await OnNavigatedToAsync(services, _cancellationTokenSource.Token);
            IsLoaded = true;
        }
        catch (Exception ex)
        {
            IsLoaded = false;

            _logger.LogError(ex, "进入视图元素时发生错误");
            _snackbarService.ShowError(ex);
        }
    }

    /// <summary>
    /// 导航离开此控件
    /// </summary>
    public async void OnNavigatedFrom()
    {
        try
        {
            if (_cancellationTokenSource is not null)
            {
                await _cancellationTokenSource.CancelAsync();
            }

            _cancellationTokenSource = new CancellationTokenSource();

            await OnNavigatedFromAsync(services, _cancellationTokenSource.Token);
            IsLoaded = true;
        }
        catch (Exception ex)
        {
            IsLoaded = false;

            _logger.LogError(ex, "离开视图元素时发生错误");
            _snackbarService.ShowError(ex);
        }
    }


    /// <summary>
    /// 导航进入此控件
    /// </summary>
    protected virtual Task OnNavigatedToAsync(IServiceProvider _, CancellationToken cancellation = default)
    {
        _logger.LogTrace("已进入视图:{type}", GetType().Name);
        return Task.CompletedTask;
    }

    /// <summary>
    /// 导航离开此控件
    /// </summary>
    protected virtual Task OnNavigatedFromAsync(IServiceProvider _, CancellationToken cancellation = default)
    {
        _logger.LogTrace("已离开视图:{type}", GetType().Name);
        return Task.CompletedTask;
    }
}
