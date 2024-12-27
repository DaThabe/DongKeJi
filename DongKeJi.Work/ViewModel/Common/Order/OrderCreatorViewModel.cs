using CommunityToolkit.Mvvm.ComponentModel;
using DongKeJi.Common.Inject;
using DongKeJi.Common.ViewModel;
using DongKeJi.Work.Model.Entity.Order;
using DongKeJi.Work.ViewModel.Common.Staff;
using Microsoft.Extensions.DependencyInjection;

namespace DongKeJi.Work.ViewModel.Common.Order;

[Inject(ServiceLifetime.Transient)]
public partial class OrderCreatorViewModel : ViewModelBase, IOrderContext
{
    /// <summary>
    ///     订单信息
    /// </summary>
    [ObservableProperty] private OrderViewModel _order = OrderViewModel.Empty;

    /// <summary>
    ///     销售选择器
    /// </summary>
    [ObservableProperty] private ListViewModel<StaffViewModel> _salesperson;

    /// <summary>
    ///     当前订单类型
    /// </summary>
    [ObservableProperty] private OrderType _currentType;


    public OrderCreatorViewModel(IEnumerable<StaffViewModel> salespersonViewModels)
    {
        Salesperson = new ListViewModel<StaffViewModel>(salespersonViewModels);
        CurrentType = OrderType.Timing;
    }

    partial void OnCurrentTypeChanged(OrderType value)
    {
        Order = value switch
        {
            OrderType.Timing => new TimingOrderViewModel
            {
                Name = "设计包月",
                Describe = Order.Describe,
                Price = 3999,
                State = OrderState.Ready,
                SubscribeTime = DateTime.Now,
                InitDays = 0,
                TotalDays = 30
            },
            OrderType.Counting => new CountingOrderViewModel
            {
                Name = "散单海报",
                Describe = Order.Describe,
                Price = 199,
                State = OrderState.Ready,
                SubscribeTime = DateTime.Now,
                InitCounts = 0,
                TotalCounts = 1
            },
            OrderType.Mixing => new MixingOrderViewModel
            {
                Name = "30天20张",
                Describe = Order.Describe,
                Price = 1999,
                State = OrderState.Ready,
                SubscribeTime = DateTime.Now,
                InitDays = 0,
                TotalDays = 30,
                InitCounts = 0,
                TotalCounts = 20
            },

            _ => Order
        };
    }
}