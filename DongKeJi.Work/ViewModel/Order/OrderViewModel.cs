using CommunityToolkit.Mvvm.ComponentModel;
using DongKeJi.ViewModel;
using DongKeJi.Work.Model.Entity.Order;

namespace DongKeJi.Work.ViewModel.Order;

/// <summary>
///     订阅订单
/// </summary>
public abstract partial class OrderViewModel : EntityViewModel, IWorkEntityViewModel
{
    /// <summary>
    /// 订单类型
    /// </summary>
    public abstract OrderType Type { get; }


    /// <summary>
    ///     描述
    /// </summary>
    [ObservableProperty] private string? _describe;


    /// <summary>
    ///     名称
    /// </summary>
    [ObservableProperty] private string _name = string.Empty;

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