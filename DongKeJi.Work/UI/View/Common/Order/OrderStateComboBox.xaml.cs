using System.Windows.Controls;
using DongKeJi.Work.Model.Entity.Order;

namespace DongKeJi.Work.UI.View.Common.Order;

/// <summary>
///     OrderStateComboBox.xaml 的交互逻辑
/// </summary>
public partial class OrderStateComboBox : UserControl
{
    public OrderStateComboBox()
    {
        InitializeComponent();

        ComboBox.ItemsSource = new[]
        {
            OrderState.Ready,
            OrderState.Active,
            OrderState.Paused,
            OrderState.Expired,
            OrderState.Cancel
        };
    }
}