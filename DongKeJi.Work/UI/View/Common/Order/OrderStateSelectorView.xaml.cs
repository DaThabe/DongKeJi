using System.Windows.Controls;
using DongKeJi.Work.Model.Entity.Order;

namespace DongKeJi.Work.UI.View.Common.Order;


/// <summary>
/// OrderStateSelectorViewModel.xaml 的交互逻辑
/// </summary>
public partial class OrderStateSelectorView : UserControl
{
    public OrderStateSelectorView()
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