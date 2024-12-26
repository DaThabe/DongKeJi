using CommunityToolkit.Mvvm.ComponentModel;
using DongKeJi.Common.ViewModel;
using DongKeJi.Work.Model.Entity.Order;
using DongKeJi.Work.ViewModel.Common.Customer;

namespace DongKeJi.Work.ViewModel.Common.Order;

/// <summary>
///     订阅订单
/// </summary>
public abstract partial class OrderViewModel : IdentifiableViewModel, IEmptyable<OrderViewModel>
{
    public abstract OrderType Type { get; }
    public bool IsEmpty => this == Empty;

    public static OrderViewModel Empty => EmptyOrderViewModel.Instance;


    /// <summary>
    ///     名称
    /// </summary>
    [ObservableProperty] private string _name = string.Empty;

    /// <summary>
    ///     描述
    /// </summary>
    [ObservableProperty] private string? _describe;

    /// <summary>
    ///     价格
    /// </summary>
    [ObservableProperty] private double _price;

    /// <summary>
    ///     状态
    /// </summary>
    [ObservableProperty] private OrderState _state = OrderState.Ready;

    /// <summary>
    ///     订阅时间
    /// </summary>
    [ObservableProperty] private DateTime _subscribeTime = DateTime.MinValue;
}

file class EmptyOrderViewModel : OrderViewModel
{
    private EmptyOrderViewModel()
    {

    }

    public override OrderType Type => OrderType.Unknown;

    public static EmptyOrderViewModel Instance { get; } = new();
}