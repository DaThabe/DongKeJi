using System.Globalization;
using System.Windows;
using System.Windows.Data;
using DongKeJi.Work.Model.Entity.Order;

namespace DongKeJi.Work.UI.Converter;

/// <summary>
///     订单类型转为名称
/// </summary>
internal class OrderTypeToNameConverter : IValueConverter
{
    public const string TimingName = "包月包年";
    public const string CountingName = "计数散单";
    public const string MixingName = "包张包天";
    public const string DefaultName = "未知";


    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not OrderType state) return DependencyProperty.UnsetValue;

        return Convert(state);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return Convert(value?.ToString() ?? "");
    }


    public static string Convert(OrderType type)
    {
        return type switch
        {
            OrderType.Timing => TimingName,
            OrderType.Counting => CountingName,
            OrderType.Mixing => MixingName,
            _ => DefaultName
        };
    }

    public static OrderType Convert(string state)
    {
        return state switch
        {
            TimingName => OrderType.Timing,
            CountingName => OrderType.Counting,
            MixingName => OrderType.Mixing,
            _ => OrderType.Unknown
        };
    }
}