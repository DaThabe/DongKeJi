using System.Windows.Controls;
using System.Windows;
using Wpf.Ui.Controls;

namespace DongKeJi.UI.Control;


/// <summary>
/// 导航页面
/// </summary>
public class NavigationPage : ContentControl, INavigationAware
{
    /// <summary>
    /// 标题
    /// </summary>
    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
        nameof(Title),
        typeof(string),
        typeof(NavigationPage),
        new PropertyMetadata(nameof(NavigationPage)));

    /// <summary>
    /// 状态
    /// </summary>
    public static readonly DependencyProperty StateProperty = DependencyProperty.Register(
            nameof(State),
            typeof(NavigationState),
            typeof(NavigationPage),
            new PropertyMetadata(NavigationState.None));

    /// <summary>
    /// 错误
    /// </summary>
    public static readonly DependencyProperty ErrorProperty = DependencyProperty.Register(
        nameof(Error),
        typeof(Exception),
        typeof(NavigationPage),
        new PropertyMetadata(null));


    /// <summary>
    /// 状态
    /// </summary>
    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <summary>
    /// 状态
    /// </summary>
    public NavigationState State
    {
        get => (NavigationState)GetValue(StateProperty);
        set => SetValue(StateProperty, value);
    }

    /// <summary>
    /// 加错误信息
    /// </summary>
    public Exception? Error
    {
        get => (Exception)GetValue(ErrorProperty);
        private set => SetValue(ErrorProperty, value);
    }



    /// <summary>
    ///     导航进入此控件
    /// </summary>
    public async void OnNavigatedTo()
    {
        try
        {
            State = NavigationState.Entering;

            if (_cancellationTokenSource is not null) await _cancellationTokenSource.CancelAsync();
            _cancellationTokenSource = new CancellationTokenSource();

            await OnNavigatedToAsync(_cancellationTokenSource.Token);
            
            State = NavigationState.Entered;
            Error = null;
        }
        catch (Exception ex)
        {
            State = NavigationState.Error;
            Error = ex;
        }
    }

    /// <summary>
    ///     导航离开此控件
    /// </summary>
    public async void OnNavigatedFrom()
    {
        try
        {
            State = NavigationState.Leaving;

            if (_cancellationTokenSource is not null) await _cancellationTokenSource.CancelAsync();
            _cancellationTokenSource = new CancellationTokenSource();

            await OnNavigatedFromAsync(_cancellationTokenSource.Token);
            
            State = NavigationState.Leaved;
            Error = null;
        }
        catch (Exception ex)
        {
            State = NavigationState.Error;
            Error = ex;
        }
    }


    /// <summary>
    ///     导航进入此控件
    /// </summary>
    protected virtual ValueTask OnNavigatedToAsync(CancellationToken cancellation = default)
    {
        return ValueTask.CompletedTask;
    }

    /// <summary>
    ///     导航离开此控件
    /// </summary>
    protected virtual ValueTask OnNavigatedFromAsync(CancellationToken cancellation = default)
    {
        return ValueTask.CompletedTask;
    }


    /// <summary>
    ///     异步导航取消
    /// </summary>
    private CancellationTokenSource? _cancellationTokenSource;
}