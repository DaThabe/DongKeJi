using System.Globalization;
using System.Windows.Data;
using Wpf.Ui.Appearance;

namespace DongKeJi.UI.Converter;

/// <summary>
///     程序主题枚举转为布尔
/// </summary>
internal class ApplicationThemeEnumToBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (parameter is not string enumString)
            throw new ArgumentException("ExceptionEnumToBooleanConverterParameterMustBeAnEnumName");

        if (!Enum.IsDefined(typeof(ApplicationTheme), value))
            throw new ArgumentException("ExceptionEnumToBooleanConverterValueMustBeAnEnum");

        var enumValue = Enum.Parse(typeof(ApplicationTheme), enumString);

        return enumValue.Equals(value);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (parameter is not string enumString)
            throw new ArgumentException("ExceptionEnumToBooleanConverterParameterMustBeAnEnumName");

        return Enum.Parse(typeof(ApplicationTheme), enumString);
    }
}