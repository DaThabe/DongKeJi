using System.Globalization;
using System.Windows;
using System.Windows.Data;
using DongKeJi.Work.Model.Entity.Order;
using Wpf.Ui.Controls;

namespace DongKeJi.Work.UI.Converter;

/// <summary>
///     订单状态转为符号
/// </summary>
internal class OrderTypeToSymbolConverter : IValueConverter
{
    private static SymbolRegular Default => SymbolRegular.ErrorCircle12;

    private static SymbolRegular Timing => SymbolRegular.Clock32;

    private static SymbolRegular Counting => SymbolRegular.LayerDiagonal20;

    private static SymbolRegular Mixing => SymbolRegular.TextBulletListSquareClock20;


    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not OrderType state) return DependencyProperty.UnsetValue;

        return state switch
        {
            OrderType.Timing => Timing,
            OrderType.Counting => Counting,
            OrderType.Mixing => Mixing,
            _ => Default
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not SymbolRegular symbol) return OrderState.None;

        return symbol switch
        {
            SymbolRegular.Clock32 => OrderType.Timing,
            SymbolRegular.LayerDiagonal20 => OrderType.Counting,
            SymbolRegular.EmojiLaugh16 => OrderType.Mixing,
            _ => OrderType.Unknown
        };
    }
}