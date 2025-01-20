using System.Windows;
using System.Windows.Controls;
using DongKeJi.UI.Control;
using DongKeJi.Work.Model.Entity.Order;

namespace DongKeJi.Work.UI.View.Order;

/// <summary>
///     OrderStateSelector.xaml 的交互逻辑
/// </summary>
public partial class OrderStateSelector : UserControl
{
    public static OrderState[] DefaultStateCollection { get; } =
    [
        OrderState.Ready,
        OrderState.Active,
        OrderState.Paused,
        OrderState.Expired,
        OrderState.Cancel
    ];


    public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
        nameof(ItemsSource),
        typeof(OrderState[]),
        typeof(OrderStateSelector),
        new PropertyMetadata(DefaultStateCollection));

    public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(
        nameof(SelectedItem),
        typeof(OrderState),
        typeof(OrderStateSelector),
        new PropertyMetadata(null));

    /// <summary>
    /// 状态集合
    /// </summary>
    public OrderState[] ItemsSource
    {
        get => (OrderState[])GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    /// <summary>
    /// 选中的状态
    /// </summary>
    public OrderState? SelectedItem
    {
        get => (OrderState?)GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }


    public OrderStateSelector()
    {
        InitializeComponent();
    }
}