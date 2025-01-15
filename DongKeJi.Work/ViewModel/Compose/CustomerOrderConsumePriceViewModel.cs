using CommunityToolkit.Mvvm.ComponentModel;
using DongKeJi.ViewModel;
using DongKeJi.Work.ViewModel.Customer;
using DongKeJi.Work.ViewModel.Order;
using DongKeJi.Work.ViewModel.Consume;
using LiveCharts;
using LiveCharts.Wpf;

namespace DongKeJi.Work.ViewModel.Compose;

/// <summary>
/// 机构订单划扣金额 (以一个订单为单位
/// </summary>
public partial class CustomerOrderConsumePriceViewModel : ObservableViewModel
{

    /// <summary>
    /// 机构信息
    /// </summary>
    public CustomerViewModel Customer { get; }

    /// <summary>
    /// 订单信息
    /// </summary>
    public OrderViewModel Order { get; }


    /// <summary>
    /// 本次划扣明细时间
    /// </summary>
    [ObservableProperty] private DateTime _yearMonthlyDate = DateTime.Now;

    /// <summary>
    /// 提成金额
    /// </summary>
    [ObservableProperty] private double _price;

    /// <summary>
    /// 划扣金额
    /// </summary>
    private double _consumePrice;

    /// <summary>
    /// 划扣总张数
    /// </summary>
    [ObservableProperty] private double _consumeTotalDays;

    /// <summary>
    /// 划扣总天数
    /// </summary>
    [ObservableProperty] private double _consumeTotalCounts;

    /// <summary>
    /// 划扣信息
    /// </summary>
    [ObservableProperty] private string _consumeMessage = "";

    /// <summary>
    /// 明细计算介绍
    /// </summary>
    [ObservableProperty] private string _calcPriceIntro = "";

    /// <summary>
    /// 图表值
    /// </summary>
    [ObservableProperty] private SeriesCollection _series = [];


    /// <summary>
    /// 机构订单划扣金额 (以一个订单为单位
    /// </summary>
    public CustomerOrderConsumePriceViewModel(
        DateTime yearMonthlyDate,
        CustomerViewModel customer,
        OrderViewModel order,
        IEnumerable<ConsumeViewModel> consumes)
    {
        Customer = customer;
        Order = order;
        YearMonthlyDate = yearMonthlyDate;

        switch (Order)
        {
            case OrderTimingViewModel t: LoadFromTiming(t); break;
            case OrderCountingViewModel c: LoadFromCounting(c); break;
            case OrderMixingViewModel m: LoadFromMixing(m); break;
        }

        Price = _consumePrice;
        return;

        //计时
        void LoadFromTiming(OrderTimingViewModel t)
        {
            var consumeTimings = consumes.OfType<ConsumeTimingViewModel>().ToArray();
            ConsumeTotalDays = consumeTimings.Sum(x => x.ConsumeDays);

            ConsumeMessage = $"{ConsumeTotalDays}天";
            CalcPriceIntro = $"(￥{t.Price}/{t.TotalDays}天)*{ConsumeTotalDays}天";
            _consumePrice = t.Price / t.TotalDays * ConsumeTotalDays;

            var format = consumeTimings.Select(x => (x.CreateTime.Day, x.ConsumeDays));
            Series.Add(CreateChartValues(format));
        }

        //计数                       
        void LoadFromCounting(OrderCountingViewModel c)
        {
            var consumeCountings = consumes.OfType<ConsumeCountingViewModel>().ToArray();
            ConsumeTotalCounts = consumeCountings.Sum(x => x.ConsumeCounts);

            ConsumeMessage = $"{ConsumeTotalCounts}张";
            CalcPriceIntro = $"(￥{c.Price}/{c.TotalCounts}张)*{ConsumeTotalCounts}张";
            _consumePrice = c.Price / c.TotalCounts * ConsumeTotalCounts;

            var format = consumeCountings.Select(x => (x.CreateTime.Day, x.ConsumeCounts));
            Series.Add(CreateChartValues(format));
        }

        //混合
        void LoadFromMixing(OrderMixingViewModel m)
        {
            var consumeMixings = consumes.OfType<ConsumeMixingViewModel>().ToArray();
            ConsumeTotalDays = consumeMixings.Sum(x => x.ConsumeDays);
            ConsumeTotalCounts = consumeMixings.Sum(x => x.ConsumeCounts);

            ConsumeMessage = $"{ConsumeTotalDays}天{ConsumeTotalCounts}张";
            CalcPriceIntro = $"(￥{m.Price}/{m.TotalCounts}天)*{ConsumeTotalCounts}天";
            _consumePrice = m.Price / m.TotalCounts * ConsumeTotalCounts;

            var format = consumeMixings.Select(x => (x.CreateTime.Day, x.ConsumeDays));
            Series.Add(CreateChartValues(format));
        }

        ColumnSeries CreateChartValues(IEnumerable<(int Day, double Value)> consumeItems)
        {
            var days = DateTime.DaysInMonth(YearMonthlyDate.Year, YearMonthlyDate.Month);
            var allDayValues = Enumerable.Range(0, days).Select(_ => 0.0).ToArray();

            foreach (var (day, value) in consumeItems)
            {
                if (day - 1 < 0 || day >= allDayValues.Length) continue;
                allDayValues[day - 1] += value;
            }

            return new ColumnSeries
            {
                Title = "消耗",
                Values = new ChartValues<double>(allDayValues)
            };
        }
    }


    /// <summary>
    /// 更新价格 所有划扣价格*提成系数
    /// </summary>
    /// <param name="coefficient">提成系数,0~1 表示0~100%</param>
    public void UpdatePrice(double coefficient)
    {
        Price = _consumePrice * coefficient;
    }
}