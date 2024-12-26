using System.Globalization;
using System.Windows;
using System.Windows.Data;
using DongKeJi.Work.Model.Entity.Order;
using DongKeJi.Work.Model.Entity.Staff;
using Wpf.Ui.Controls;

namespace DongKeJi.Work.UI.Converter;

/// <summary>
///     职位转为符号
/// </summary>
internal class StaffPositionTypeToSymbolConverter : IValueConverter
{
    private static SymbolRegular Default { get; } = SymbolRegular.Empty;

    private static SymbolRegular Designer { get; } = SymbolRegular.DesignIdeas16;

    private static SymbolRegular Salesperson { get; } = SymbolRegular.BookmarkAdd24;


    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not StaffPositionType type) return DependencyProperty.UnsetValue;

        return type switch
        {
            StaffPositionType.Designer => Designer,
            StaffPositionType.Salesperson => Salesperson,
            _ => Default
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not SymbolRegular symbol) return OrderState.None;

        return symbol switch
        {
            SymbolRegular.DesignIdeas16 => StaffPositionType.Designer,
            SymbolRegular.BookmarkAdd24 => StaffPositionType.Salesperson,
            _ => StaffPositionType.None
        };
    }
}