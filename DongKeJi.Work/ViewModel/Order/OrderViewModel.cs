using CommunityToolkit.Mvvm.ComponentModel;
using DongKeJi.ViewModel;
using DongKeJi.Work.Model.Entity.Order;
using System.ComponentModel.DataAnnotations;

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
    ///     名称
    /// </summary>
    [ObservableProperty]
    [Required(ErrorMessage = "订单名称不可为空")]
    [RegularExpression(@"^[^\s\\][^\x5C]*$", ErrorMessage = "订单名称首字符不可为空且不可使用转义字符")]
    [MinLength(1, ErrorMessage = "订单名称长度不可小于1")]
    [MaxLength(128, ErrorMessage = "订单名称长度不可大于128")]
    private string _name = string.Empty;

    /// <summary>
    ///     描述
    /// </summary>
    [ObservableProperty] private string? _describe;

    /// <summary>
    ///     价格
    /// </summary>
    [ObservableProperty]
    [Required(ErrorMessage = "订单价格不可为空")]
    [Range(1, 99999, ErrorMessage = "订单价格需要>=1 且 < 99999")] 
    private double _price;

    /// <summary>
    ///     状态
    /// </summary>
    [ObservableProperty] private OrderState _state = OrderState.Ready;

    /// <summary>
    ///     订阅时间
    /// </summary>
    [ObservableProperty] private DateTime _subscribeTime = DateTime.MinValue;



    partial void OnNameChanging(string value) => ValidateProperty(value, nameof(Name));

    partial void OnDescribeChanging(string? value) => ValidateProperty(value, nameof(Describe));

    partial void OnPriceChanging(double value) => ValidateProperty(value, nameof(Describe));

    partial void OnStateChanging(OrderState value) => ValidateProperty(value, nameof(State));

    partial void OnSubscribeTimeChanging(DateTime value) => ValidateProperty(value, nameof(SubscribeTime));
}