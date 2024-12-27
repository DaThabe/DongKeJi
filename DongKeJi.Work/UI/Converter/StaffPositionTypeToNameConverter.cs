using System.Globalization;
using System.Windows;
using System.Windows.Data;
using DongKeJi.Work.Model.Entity.Staff;

namespace DongKeJi.Work.UI.Converter;

/// <summary>
///     职位转为名称
/// </summary>
internal class StaffPositionTypeToNameConverter : IValueConverter
{
    private const string Designer = "设计";

    private const string Salesperson = "销售";


    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not StaffPositionType type) return DependencyProperty.UnsetValue;

        return type switch
        {
            StaffPositionType.Designer => Designer,
            StaffPositionType.Salesperson => Salesperson,
            _ => DependencyProperty.UnsetValue
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string name) return StaffPositionType.None;

        return name switch
        {
            Designer => StaffPositionType.Designer,
            Salesperson => StaffPositionType.Salesperson,
            _ => StaffPositionType.None
        };
    }
}