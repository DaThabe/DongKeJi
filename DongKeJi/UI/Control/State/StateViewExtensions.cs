using System.Windows;
using System.Windows.Threading;

namespace DongKeJi.UI.Control.State;

public static class StateViewExtensions
{
    /// <summary>
    /// 开始加载 并指定进度
    /// </summary>
    /// <param name="control"></param>
    /// <param name="action"></param>
    public static void OnLoading(this StateView control, Action<ILoadingProgress> action)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            var loadingView = new ProgressBarView();
            control.StateContent = loadingView;

            action(loadingView);
            control.StateContent = null;

        }, DispatcherPriority.Render);
    }

    /// <summary>
    /// 开始加载 并指定进度
    /// </summary>
    /// <param name="control"></param>
    /// <param name="action"></param>
    public static void OnLoading(this StateView control, Func<ILoadingProgress, ValueTask> action)
    {
        Application.Current.Dispatcher.InvokeAsync(async () =>
        {
            var loadingView = new ProgressBarView();
            control.StateContent = loadingView;

            await action(loadingView);
            control.StateContent = null;

        }, DispatcherPriority.Render);
    }

    /// <summary>
    /// 开始加载
    /// </summary>
    /// <param name="control"></param>
    /// <param name="action"></param>
    public static void OnLoading(this StateView control, Func<ValueTask> action)
    {
        Application.Current.Dispatcher.InvokeAsync(async () =>
        {
            control.StateContent = new ProgressBarView();
            await action();
            control.StateContent = null;

        }, DispatcherPriority.Render);
    }

    /// <summary>
    /// 开始加载
    /// </summary>
    /// <param name="control"></param>
    /// <param name="action"></param>
    public static void OnLoading(this StateView control, Action action)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            control.StateContent = new ProgressBarView();
            action();
            control.StateContent = null;

        }, DispatcherPriority.Render);
    }
}