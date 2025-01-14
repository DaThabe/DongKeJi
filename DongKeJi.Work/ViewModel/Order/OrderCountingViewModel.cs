using CommunityToolkit.Mvvm.ComponentModel;
using DongKeJi.Work.Model.Entity.Order;
using System.ComponentModel.DataAnnotations;

namespace DongKeJi.Work.ViewModel.Order;

/// <summary>
///     计数订单
/// </summary>
public partial class OrderCountingViewModel : OrderViewModel
{
    public override OrderType Type => OrderType.Counting;

    /// <summary>
    ///     初始张数
    /// </summary>
    [ObservableProperty]
    [Required(ErrorMessage = "初始天数不可为空")]
    [Range(0, 99999, ErrorMessage = "初始天数需要>=0 且 < 99999")] 
    private double _initCounts;

    /// <summary>
    ///     总张数
    /// </summary>
    [ObservableProperty]
    [Required(ErrorMessage = "总天数不可为空")]
    [Range(1, 99999, ErrorMessage = "总天数需要>=0 且 < 99999")] 
    private double _totalCounts;


    partial void OnInitCountsChanging(double value) => ValidateProperty(value, nameof(InitCounts));

    partial void OnTotalCountsChanging(double value) => ValidateProperty(value, nameof(TotalCounts));

    public override string ToString()
    {
        return $"{base.ToString()}, 初始{InitCounts}张, 总{TotalCounts}张";
    }
}