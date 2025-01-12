using System.Windows.Controls;
using System.Windows;
using Wpf.Ui.Controls;

namespace DongKeJi.UI.View;


/// <summary>
/// 导航视图
/// </summary>
public class NavigationView : ContentControl, INavigationAware
{
    /// <summary>
    /// 标题
    /// </summary>
    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
        nameof(Title),
        typeof(string),
        typeof(NavigationView),
        new PropertyMetadata(nameof(NavigationView)));

    /// <summary>
    /// 状态
    /// </summary>
    public static readonly DependencyProperty StateProperty = DependencyProperty.Register(
            nameof(State),
            typeof(NavigationViewState),
            typeof(NavigationView),
            new PropertyMetadata(NavigationViewState.None));

    /// <summary>
    /// 错误
    /// </summary>
    public static readonly DependencyProperty ErrorProperty = DependencyProperty.Register(
        nameof(Error),
        typeof(Exception),
        typeof(NavigationView),
        new PropertyMetadata(null));


    public NavigationView()
    {
        var resourceDictionary = new ResourceDictionary
        {
            Source = new Uri("pack://application:,,,/DongKeJi;component/UI/Themes/Generic.xaml")
        };

        Template = resourceDictionary["NavigationViewTemplate"] as ControlTemplate;
    }


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
    public NavigationViewState State
    {
        get => (NavigationViewState)GetValue(StateProperty);
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
            State = NavigationViewState.Entering;

            if (_cancellationTokenSource is not null) await _cancellationTokenSource.CancelAsync();
            _cancellationTokenSource = new CancellationTokenSource();

            await OnNavigatedToAsync(_cancellationTokenSource.Token);
            
            State = NavigationViewState.Focused;
            Error = null;
        }
        catch (Exception ex)
        {
            State = NavigationViewState.Error;
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
            State = NavigationViewState.Leaving;

            if (_cancellationTokenSource is not null) await _cancellationTokenSource.CancelAsync();
            _cancellationTokenSource = new CancellationTokenSource();

            await OnNavigatedFromAsync(_cancellationTokenSource.Token);
            
            State = NavigationViewState.Unfocused;
            Error = null;
        }
        catch (Exception ex)
        {
            State = NavigationViewState.Error;
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


public enum NavigationViewState
{
    None,
    Entering,
    Leaving,
    Focused,
    Unfocused,
    Error
}