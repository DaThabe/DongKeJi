using CommunityToolkit.Mvvm.ComponentModel;
using DongKeJi.Work.Model.Entity.Order;

namespace DongKeJi.Work.ViewModel.Common.Order;

/// <summary>
///     计数订单
/// </summary>
public partial class CountingOrderViewModel : OrderViewModel
{
    /// <summary>
    ///     初始张数
    /// </summary>
    [ObservableProperty] private double _initCounts;

    /// <summary>
    ///     总张数
    /// </summary>
    [ObservableProperty] private double _totalCounts;

    public override OrderType Type => OrderType.Counting;
}