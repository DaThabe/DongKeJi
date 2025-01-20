using System.Windows;
using System.Windows.Threading;

namespace DongKeJi.UI.Control.State;


/// <summary>
/// 进度条视图
/// </summary>
public partial class ProgressBarView : ILoadingProgress
{
    /// <summary>
    /// 动画时长
    /// </summary>
    public static TimeSpan AnimeTime { get; set; } = TimeSpan.FromSeconds(0.3);

    /// <summary>
    /// 动画帧率
    /// </summary>
    public static double FrameRate { get; set; } = 60.0;



    public ProgressBarView()
    {
        InitializeComponent();
    }

    public void UpdateProgress(Action<ProgressValue> update)
    {
        var oldValue = _currentValue.Value;
        update(_currentValue);

        TitleTextBlock.Text = _currentValue.Title;
        DescribeTextBlock.Text = _currentValue.Describe;

        if (_currentValue.Value is null)
        {
            this.ProgressRing.IsIndeterminate = true;
            return;
        }

        var end = 360 * (double)_currentValue.Value - 1;
        var start = 360 * oldValue ?? 0;

        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = new CancellationTokenSource();
        SetValue(start, end, AnimeTime, _cancellationTokenSource.Token);
    }

    private void SetValue(double start, double end, TimeSpan time, CancellationToken cancellation = default)
    {
        
        // 总帧数
        var totalFrames = FrameRate * time.TotalSeconds;

        // 每帧的增量（正值或负值）
        var increment = (end - start) / totalFrames;

        // 每帧的延迟时间 (毫秒)
        var delay = (int)(time.TotalMilliseconds / totalFrames);

        // 确保更新在 UI 线程
        Application.Current.Dispatcher.InvokeAsync(async () =>
        {
            var currentValue = start;

            // 判断递增或递减方向
            if (start < end)
            {
                // 递增
                while (currentValue < end)
                {
                    ProgressRing.EngAngle = currentValue;
                    currentValue += increment;

                    await Task.Delay(delay);
                }
            }
            else
            {
                // 递减
                while (currentValue > end)
                {
                    ProgressRing.EngAngle = currentValue;
                    currentValue += increment;

                    await Task.Delay(delay);
                }
            }

            // 确保最终值准确
            ProgressRing.EngAngle = end;

        }, DispatcherPriority.Render, cancellation);
    }



    private CancellationTokenSource? _cancellationTokenSource;

    private readonly ProgressValue _currentValue = new();
}




/// <summary>
/// 加载进度
/// </summary>
public interface ILoadingProgress
{
    /// <summary>
    /// 更新进度
    /// </summary>
    /// <param name="update"></param>
    void UpdateProgress(Action<ProgressValue> update);
}

public static class LoadingProgressExtensions
{
    /// <summary>
    /// 更新进度, 没有确定的进度, 但是可以提示一下当前的描述
    /// </summary>
    /// <param name="loadingProgress"></param>
    /// <param name="describe"></param>
    /// <param name="title"></param>
    public static void UpdateProgress(this ILoadingProgress loadingProgress, string describe, string? title = null)
    {
        loadingProgress.UpdateProgress(x =>
        {
            x.Title = title ?? x.Title;
            x.Describe = describe;
            x.Value = null;
        });
    }


    /// <summary>
    /// 更新进度
    /// </summary>
    /// <param name="loadingProgress"></param>
    /// <param name="value"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    public static void UpdateProgress(this ILoadingProgress loadingProgress, int? value = null, int min = 0, int max = 100)
    {
        loadingProgress.UpdateProgress(x =>
        {
            x.Value = value is null ? null : (double)(value - min) / (max - min);
        });
    }

    /// <summary>
    /// 更新进度 并指定标题和描述
    /// </summary>
    /// <param name="loadingProgress"></param>
    /// <param name="value"></param>
    /// <param name="describe"></param>
    /// <param name="title"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    public static void UpdateProgress(this ILoadingProgress loadingProgress, int? value, string describe, string? title = null, int min = 0, int max = 100)
    {
        loadingProgress.UpdateProgress(x =>
        {
            x.Title = title ?? x.Title;
            x.Describe = describe;
            x.Value = value is null ? null : (double)(value - min) / (max - min);
        });
    }
}

/// <summary>
/// 进度值
/// </summary>
public class ProgressValue
{
    /// <summary>
    /// 当前进度标题
    /// </summary>
    public string Title { get; set; } = "加载中...";
    
    /// <summary>
    /// 当前进度描述
    /// </summary>
    public string? Describe { get; set; }

    /// <summary>
    /// 进度完成的百分比 0~1
    /// </summary>
    public double? Value { get; set; }
}