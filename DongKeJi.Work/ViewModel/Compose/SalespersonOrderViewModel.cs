using CommunityToolkit.Mvvm.ComponentModel;
using DongKeJi.ViewModel;
using DongKeJi.Work.ViewModel.Order;
using DongKeJi.Work.ViewModel.Staff;

namespace DongKeJi.Work.ViewModel.Compose;


/// <summary>
///  销售订单
/// </summary>
public partial class SalespersonOrderViewModel(
    StaffViewModel salesperson,
    OrderViewModel order
    ) : ObservableViewModel
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