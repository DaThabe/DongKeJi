using CommunityToolkit.Mvvm.ComponentModel;
using DongKeJi.Work.Model.Entity.Order;

namespace DongKeJi.Work.ViewModel.Order;

/// <summary>
///     计数订阅订单
/// </summary>
public partial class OrderMixingViewModel : OrderViewModel
{
    /// <summary>
    ///     初始张数
    /// </summary>
    [ObservableProperty] private double _initCounts;

    /// <summary>
    ///     初始天数
    /// </summary>
    [ObservableProperty] private double _initDays;


    /// <summary>
    ///     总张数
    /// </summary>
    [ObservableProperty] private double _totalCounts;

    /// <summary>
    ///     总天数
    /// </summary>
    [ObservableProperty] private double _totalDays;

    public override OrderType Type => OrderType.Mixing;
}