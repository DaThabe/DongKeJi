using System.Windows;
using System.Windows.Controls;
using DongKeJi.Work.Model.Entity.Order;
using DongKeJi.Work.ViewModel.Staff;

namespace DongKeJi.Work.UI.View.Staff;

/// <summary>
///     OrderStateSelector.xaml 的交互逻辑
/// </summary>
public partial class StaffSelector : UserControl
{
    public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
        nameof(ItemsSource),
        typeof(StaffViewModel[]),
        typeof(StaffSelector),
        new PropertyMetadata(Array.Empty<StaffViewModel>()));

    public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(
        nameof(SelectedItem),
        typeof(OrderState),
        typeof(StaffSelector),
        new PropertyMetadata(null));


    public StaffViewModel[] ItemsSource
    {
        get => (StaffViewModel[])GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public StaffViewModel? SelectedItem
    {
        get => (StaffViewModel?)GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    public StaffSelector()
    {
        InitializeComponent();
    }
}