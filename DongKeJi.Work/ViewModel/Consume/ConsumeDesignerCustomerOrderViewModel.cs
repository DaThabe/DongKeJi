using CommunityToolkit.Mvvm.ComponentModel;
using DongKeJi.ViewModel;
using DongKeJi.Work.ViewModel.Customer;
using DongKeJi.Work.ViewModel.Order;
using DongKeJi.Work.ViewModel.Staff;

namespace DongKeJi.Work.ViewModel.Consume;

/// <summary>
/// 设计师机构订单划扣
/// </summary>
/// <param name="customer"></param>
/// <param name="order"></param>
/// <param name="designer"></param>
/// <param name="consume"></param>
public partial class ConsumeDesignerCustomerOrderViewModel(
    ConsumeViewModel consume,
    StaffViewModel designer, 
    CustomerViewModel customer, 
    OrderViewModel order
) : ObservableViewModel
{
    /// <summary>
    /// 设计师
    /// </summary>
    [ObservableProperty] private StaffViewModel _designer = designer;

    /// <summary>
    /// 机构
    /// </summary>
    [ObservableProperty] private CustomerViewModel _customer = customer;

    /// <summary>
    /// 订单
    /// </summary>
    [ObservableProperty] private OrderViewModel _order = order;
    
    /// <summary>
    /// 划扣
    /// </summary>
    [ObservableProperty] private ConsumeViewModel _consume = consume;


    public override string ToString()
    {
        return $"{Designer.Name}-{Customer.Name}-{Order.Name}-{Consume.Title}";
    }
}