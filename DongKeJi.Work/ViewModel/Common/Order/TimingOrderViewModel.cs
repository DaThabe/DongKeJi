using CommunityToolkit.Mvvm.ComponentModel;
using DongKeJi.Work.Model.Entity.Order;

namespace DongKeJi.Work.ViewModel.Common.Order;

/// <summary>
///     计时订阅订单
/// </summary>
public partial class TimingOrderViewModel : OrderViewModel
{
    public override OrderType Type => OrderType.Timing;

    /// <summary>
    ///     初始天数
    /// </summary>
    [ObservableProperty] private double _initDays;


    /// <summary>
    ///     总天数
    /// </summary>
    [ObservableProperty] private double _totalDays;
}