using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DongKeJi.UI.Converter;

/// <summary>
///     布尔转为可见度 (false可见, true不可见
/// </summary>
public class InverseBooleanToVisibilityConverter : IValueConverter
{
    public Visibility Collapsed { get; set; } = Visibility.Collapsed;
    public Visibility Visible { get; set; } = Visibility.Visible;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool b)
            // false -> Visible, true -> Collapsed/Hidden
            return b ? Collapsed : Visible;

        // 默认返回 Collapsed，处理非 bool 值情况
        return Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Visibility visibility)
            // Visible -> false (可见 -> false), Collapsed/Hidden -> true (不可见 -> true)
            return visibility != Visibility.Visible;

        // 如果转换失败，返回 true 即不可见
        return true;
    }
}