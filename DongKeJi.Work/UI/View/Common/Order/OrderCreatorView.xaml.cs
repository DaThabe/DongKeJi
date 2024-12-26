using System.Windows.Controls;
using DongKeJi.Work.Model.Entity.Order;

namespace DongKeJi.Work.UI.View.Common.Order;

/// <summary>
///     OrderCreatorView.xaml 的交互逻辑
/// </summary>
public partial class OrderCreatorView : UserControl
{
    public OrderCreatorView()
    {
        InitializeComponent();

        OrderTypeComboBox.ItemsSource = new[]
        {
            OrderType.Timing,
            OrderType.Counting,
            OrderType.Mixing
        };
    }
}