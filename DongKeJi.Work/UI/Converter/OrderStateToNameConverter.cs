using System.Globalization;
using System.Windows;
using System.Windows.Data;
using DongKeJi.Work.Model.Entity.Order;

namespace DongKeJi.Work.UI.Converter;

/// <summary>
///     订单状态转为名称
/// </summary>
internal class OrderStateToNameConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not OrderState state) return DependencyProperty.UnsetValue;

        return Convert(state);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return Convert(value?.ToString() ?? "");
    }


    public static string Convert(OrderState state)
    {
        return state switch
        {
            OrderState.Ready => "准备",
            OrderState.Active => "活跃",
            OrderState.Paused => "暂停",
            OrderState.Expired => "过期",
            OrderState.Cancel => "取消",
            _ => "错误"
        };
    }

    public static OrderState Convert(string state)
    {
        return state switch
        {
            "准备" => OrderState.Ready,
            "活跃" => OrderState.Active,
            "暂停" => OrderState.Paused,
            "过期" => OrderState.Expired,
            "取消" => OrderState.Cancel,
            _ => OrderState.None
        };
    }
}