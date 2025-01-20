using System.Windows;
using System.Windows.Controls;

namespace DongKeJi.UI.Control.State;

/// <summary>
/// 状态视图
/// </summary>
public class StateView : ContentControl
{
    /// <summary>
    /// 内容可见性
    /// </summary>
    public static readonly DependencyProperty ContentVisibilityProperty = DependencyProperty.Register(
        nameof(ContentVisibility),
        typeof(Visibility),
        typeof(StateView),
        new PropertyMetadata(Visibility.Visible));

    /// <summary>
    /// 内容可见性
    /// </summary>
    public static readonly DependencyProperty StateVisibilityProperty = DependencyProperty.Register(
        nameof(StateVisibility),
        typeof(Visibility),
        typeof(StateView),
        new PropertyMetadata(Visibility.Collapsed));

    /// <summary>
    /// 状态内容
    /// </summary>
    public static readonly DependencyProperty StateContentProperty = DependencyProperty.Register(
        nameof(StateContent),
        typeof(object),
        typeof(StateView),
        new PropertyMetadata(null, PropertyChangedCallback));


    /// <summary>
    /// 内容可见性
    /// </summary>
    public Visibility ContentVisibility
    {
        get => (Visibility)GetValue(ContentVisibilityProperty);
        set => SetValue(ContentVisibilityProperty, value);
    }

    /// <summary>
    /// 状态可见性
    /// </summary>
    public Visibility StateVisibility
    {
        get => (Visibility)GetValue(StateVisibilityProperty);
        set => SetValue(StateVisibilityProperty, value);
    }

    /// <summary>
    /// 状态内容
    /// </summary>
    public object? StateContent
    {
        get => (object?)GetValue(StateContentProperty);
        set => SetValue(StateContentProperty, value);
    }


    private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not StateView stateView) return;

        var stateIsEmpty = e.NewValue is null;
        stateView.StateVisibility = stateIsEmpty ? Visibility.Collapsed : Visibility.Visible;
        stateView.ContentVisibility = stateIsEmpty ? Visibility.Visible : Visibility.Collapsed;
    }
}