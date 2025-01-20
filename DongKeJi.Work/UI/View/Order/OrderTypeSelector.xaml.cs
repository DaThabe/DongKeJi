using System.Windows;
using DongKeJi.Work.Model.Entity.Order;

namespace DongKeJi.Work.UI.View.Order;

/// <summary>
///     OrderStateSelector.xaml 的交互逻辑
/// </summary>
public partial class OrderTypeSelector
{
    public static OrderType[] DefaultStateCollection { get; } =
    [
        OrderType.Timing,
        OrderType.Counting,
        OrderType.Mixing
    ];


    public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
        nameof(ItemsSource),
        typeof(OrderType[]),
        typeof(OrderTypeSelector),
        new PropertyMetadata(DefaultStateCollection));

    public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(
        nameof(SelectedItem),
        typeof(OrderType),
        typeof(OrderTypeSelector),
        new PropertyMetadata(null));
    public OrderType[] ItemsSource
    {
        get => (OrderType[])GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public OrderType? SelectedItem
    {
        get => (OrderType?)GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }


    public OrderTypeSelector()
    {
        InitializeComponent();
    }
}