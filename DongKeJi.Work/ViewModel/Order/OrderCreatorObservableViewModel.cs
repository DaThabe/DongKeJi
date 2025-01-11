using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using DongKeJi.Extensions;
using DongKeJi.Inject;
using DongKeJi.ViewModel;
using DongKeJi.Work.Model.Entity.Order;
using DongKeJi.Work.ViewModel.Staff;
using Microsoft.Extensions.DependencyInjection;

namespace DongKeJi.Work.ViewModel.Order;


[Inject(ServiceLifetime.Transient)]
public partial class OrderCreatorObservableViewModel : DongKeJi.ViewModel.ObservableViewModel
{
    /// <summary>
    ///     订单信息
    /// </summary>
    [ObservableProperty] private OrderViewModel _order = OrderViewModel.Default;

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


    public OrderCreatorObservableViewModel(IEnumerable<StaffViewModel> salespersonViewModels)
    {
        SalespersonSet = salespersonViewModels.ToObservableCollection();
        Salesperson = Enumerable.FirstOrDefault<StaffViewModel>(SalespersonSet);

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