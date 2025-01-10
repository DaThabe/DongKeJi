using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using DongKeJi.Common.Extensions;
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
    ///     当前选择的销售
    /// </summary>
    [ObservableProperty] private StaffViewModel? _salesperson;

    /// <summary>
    ///     销售列表
    /// </summary>
    [ObservableProperty] private ObservableCollection<StaffViewModel> _salespersonSet;

    /// <summary>
    ///     当前订单类型
    /// </summary>
    [ObservableProperty] private OrderType _currentType;


    public OrderCreatorViewModel(IEnumerable<StaffViewModel> salespersonViewModels)
    {
        SalespersonSet = salespersonViewModels.ToObservableCollection();
        Salesperson = SalespersonSet.FirstOrDefault();

        CurrentType = OrderType.Timing;
    }

    partial void OnCurrentTypeChanged(OrderType value)
    {
        Order = value switch
        {
            OrderType.Timing => new OrderTimingViewModel
            {
                Name = "设计包月",
                Describe = Order.Describe,
                Price = 3999,
                State = OrderState.Ready,
                SubscribeTime = DateTime.Now,
                InitDays = 0,
                TotalDays = 30
            },
            OrderType.Counting => new OrderCountingViewModel
            {
                Name = "散单海报",
                Describe = Order.Describe,
                Price = 199,
                State = OrderState.Ready,
                SubscribeTime = DateTime.Now,
                InitCounts = 0,
                TotalCounts = 1
            },
            OrderType.Mixing => new OrderMixingViewModel
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