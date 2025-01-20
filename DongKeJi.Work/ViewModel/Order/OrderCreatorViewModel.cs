using CommunityToolkit.Mvvm.ComponentModel;
using DongKeJi.Inject;
using DongKeJi.Work.Model.Entity.Order;
using DongKeJi.Work.ViewModel.Staff;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;

namespace DongKeJi.Work.ViewModel.Order;


[Inject(ServiceLifetime.Transient)]
public partial class OrderCreatorViewModel : OrderEditViewModel
{
    /// <summary>
    /// 创建
    /// </summary>
    /// <param name="salespersons"></param>
    /// <returns></returns>
    public new static OrderCreatorViewModel Create(ObservableCollection<StaffViewModel> salespersons)
    {
        var creator = new OrderCreatorViewModel
        {
            SalespersonSelector = new StaffSelectorViewModel()
            {
                ItemsSource = salespersons,
                Selected = salespersons.FirstOrDefault()
            },
            CurrentType = OrderType.Timing
        };

        creator.SalespersonSelector.Selected = creator.SalespersonSelector.ItemsSource.FirstOrDefault();
        //creator.OnCurrentTypeChanged(OrderType.Timing);

        return creator;
    }


    /// <summary>
    ///     当前订单类型
    /// </summary>
    [ObservableProperty] 
    private OrderType _currentType = OrderType.Unknown;


    partial void OnCurrentTypeChanged(OrderType value)
    {
        Order = value switch
        {
            OrderType.Timing => new OrderTimingViewModel
            {
                Name = "设计包月",
                Describe = Order.Describe,
                Price = 3999,
                State = OrderState.Active,
                SubscribeTime = DateTime.Now,
                InitDays = 0,
                TotalDays = 30
            },
            OrderType.Counting => new OrderCountingViewModel
            {
                Name = "散单海报",
                Describe = Order.Describe,
                Price = 199,
                State = OrderState.Active,
                SubscribeTime = DateTime.Now,
                InitCounts = 0,
                TotalCounts = 1
            },
            OrderType.Mixing => new OrderMixingViewModel
            {
                Name = "30天20张",
                Describe = Order.Describe,
                Price = 1999,
                State = OrderState.Active,
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