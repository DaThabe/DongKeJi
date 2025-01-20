﻿using System.Windows.Controls;
using System.Windows;
using Wpf.Ui.Controls;

namespace DongKeJi.UI.Control;

/// <summary>
/// 导航页面
/// </summary>
[Obsolete]
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
            if (_cancellationTokenSource is not null) await _cancellationTokenSource.CancelAsync();
            _cancellationTokenSource = new CancellationTokenSource();

            await InternalNavigateToAsync(_cancellationTokenSource.Token);
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
            if (_cancellationTokenSource is not null) await _cancellationTokenSource.CancelAsync();
            _cancellationTokenSource = new CancellationTokenSource();

            await InternalNavigateFromAsync(_cancellationTokenSource.Token);
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
    protected virtual ValueTask InternalNavigateToAsync(CancellationToken cancellation = default)
    {
        return ValueTask.CompletedTask;
    }

    /// <summary>
    ///     导航离开此控件
    /// </summary>
    protected virtual  ValueTask InternalNavigateFromAsync(CancellationToken cancellation = default)
    {
        return ValueTask.CompletedTask;
    }




    /// <summary>
    ///     异步导航取消
    /// </summary>
    private CancellationTokenSource? _cancellationTokenSource;
}