using System.Globalization;
using System.Windows.Data;
using DongKeJi.Work.Model.Entity.Order;
using Wpf.Ui.Controls;

namespace DongKeJi.Work.UI.Converter;

/// <summary>
///     订单状态转为符号
/// </summary>
internal class OrderStateToSymbolConverter : IValueConverter
{
    private static SymbolRegular Default { get; } = SymbolRegular.EmojiSadSlight24;

    private static SymbolRegular Ready { get; } = SymbolRegular.MoreHorizontal32;

    private static SymbolRegular Active { get; } = SymbolRegular.Play32;

    private static SymbolRegular Paused { get; } = SymbolRegular.Pause32;

    private static SymbolRegular Expired { get; } = SymbolRegular.ClockDismiss20;

    private static SymbolRegular Cancel { get; } = SymbolRegular.DismissCircle32;


    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not OrderState state) return Default;

        return state switch
        {
            OrderState.Ready => Ready,
            OrderState.Active => Active,
            OrderState.Paused => Paused,
            OrderState.Expired => Expired,
            OrderState.Cancel => Cancel,
            _ => Default
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not SymbolRegular symbol) return OrderState.None;

        return symbol switch
        {
            SymbolRegular.MoreHorizontal32 => OrderState.Ready,
            SymbolRegular.Play32 => OrderState.Active,
            SymbolRegular.Pause32 => OrderState.Paused,
            SymbolRegular.ClockDismiss20 => OrderState.Expired,
            SymbolRegular.DismissCircle32 => OrderState.Cancel,
            _ => OrderState.None
        };
    }
}