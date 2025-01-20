using System.Windows;
using DongKeJi.Work.Model.Entity.Staff;
using DongKeJi.Work.UI.View.Order;

namespace DongKeJi.Work.UI.View.Staff;

/// <summary>
///     OrderStateSelectorViewModel.xaml 的交互逻辑
/// </summary>
public partial class StaffPositionTypeSelector
{
    public static StaffPositionType[] DefaultTypeCollection { get; } =
    [
        StaffPositionType.Designer,
        StaffPositionType.Salesperson
    ];


    public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
        nameof(ItemsSource),
        typeof(StaffPositionType[]),
        typeof(StaffPositionTypeSelector),
        new PropertyMetadata(DefaultTypeCollection));

    public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(
        nameof(SelectedItem),
        typeof(StaffPositionType),
        typeof(StaffPositionTypeSelector),
        new PropertyMetadata(null));


    public StaffPositionType[] ItemsSource
    {
        get => (StaffPositionType[])GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public StaffPositionType? SelectedItem
    {
        get => (StaffPositionType?)GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    public StaffPositionTypeSelector()
    {
        InitializeComponent();
    }
}