using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DongKeJi.Common.UI.Converter;


/// <summary>
/// 空值转为可见性  空则隐藏 相反显示
/// </summary>
public class NullToVisibilityConverter : IValueConverter
{
    public Visibility Collapsed { get; set; } = Visibility.Collapsed;
    public Visibility Visible { get; set; } = Visibility.Visible;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value == null ? Collapsed : Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        //TODO: 单向转换
        return DependencyProperty.UnsetValue;
    }
}
