using CommunityToolkit.Mvvm.ComponentModel;
using DongKeJi.ViewModel;
using DongKeJi.Work.Model.Entity.Order;

namespace DongKeJi.Work.ViewModel.Compose;

/// <summary>
/// 
/// </summary>
public partial class MonthlyCustomerConsumePrice : ObservableViewModel
{
    /// <summary>
    /// 计时划扣信息
    /// </summary>
    [ObservableProperty] private string _timingConsumeMessage = "";

    /// <summary>
    /// 计时总提成
    /// </summary>
    [ObservableProperty] private double _timingPrice;

    /// <summary>
    /// 计数划扣信息
    /// </summary>
    [ObservableProperty] private string _countConsumeMessage = "";

    /// <summary>
    /// 计数总提成
    /// </summary>
    [ObservableProperty] private double _countPrice;

    /// <summary>
    /// 混合划扣信息
    /// </summary>
    [ObservableProperty] private string _mixingConsumeMessage = "";

    /// <summary>
    /// 混合总提成
    /// </summary>
    [ObservableProperty] private double _mixingPrice;

    /// <summary>
    /// 总提成
    /// </summary>
    [ObservableProperty] private double _price;

    /// <summary>
    /// 
    /// </summary>
    public MonthlyCustomerConsumePrice(IEnumerable<CustomerOrderConsumePriceViewModel> items)
    {
        var timingValue = 0.0;
        var countValue = 0.0;
        var mixingValue = 0.0;

        foreach (var item in items)
        {
            if (item.Order.Type == OrderType.Timing)
            {
                timingValue += item.ConsumeTotalDays;
                _consumeTimingPrice += item.Price;
            }
            else if (item.Order.Type == OrderType.Counting)
            {
                countValue += item.ConsumeTotalCounts;
                _consumeCountPrice += item.Price;
            }
            else if (item.Order.Type == OrderType.Mixing)
            {
                mixingValue += item.ConsumeTotalCounts;
                _consumeMixingPrice += item.Price;
            }
        }

        _consumePrice = _consumeTimingPrice + _consumeCountPrice + _consumeMixingPrice;

        TimingConsumeMessage = $"{timingValue}天";
        CountConsumeMessage = $"{countValue}张";
        MixingConsumeMessage = $"{mixingValue}张";


        UpdatePrice(1.0);
    }

    /// <summary>
    /// 更新价格 所有划扣价格*提成系数
    /// </summary>
    /// <param name="coefficient">提成系数,0~1 表示0~100%</param>
    public void UpdatePrice(double coefficient)
    {
        TimingPrice = _consumeTimingPrice * coefficient;
        CountPrice = _consumeCountPrice * coefficient;
        MixingPrice = _consumeMixingPrice * coefficient;
        Price = _consumePrice * coefficient;
    }


    private readonly double _consumeTimingPrice;
    private readonly double _consumeCountPrice;
    private readonly double _consumeMixingPrice;
    private readonly double _consumePrice;
}