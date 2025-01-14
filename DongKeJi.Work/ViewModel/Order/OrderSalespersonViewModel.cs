using CommunityToolkit.Mvvm.ComponentModel;
using DongKeJi.ViewModel;
using DongKeJi.Work.ViewModel.Staff;

namespace DongKeJi.Work.ViewModel.Order;


/// <summary>
///     订单销售
/// </summary>
public partial class OrderSalespersonViewModel(
    OrderViewModel order, 
    StaffViewModel salesperson) : ObservableViewModel
{
    public event Action<StaffViewModel>?  SalespersonChanged; 

    /// <summary>
    /// 订单
    /// </summary>
    [ObservableProperty] private OrderViewModel _order = order;

    /// <summary>
    /// 销售
    /// </summary>
    [ObservableProperty] private StaffViewModel _salesperson = salesperson;


    partial void OnSalespersonChanged(StaffViewModel value)
    {
        SalespersonChanged?.Invoke(value);
    }

    public override string ToString()
    {
        return $"{Order.Name}-{Salesperson.Name}";
    }
}