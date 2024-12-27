using System.Globalization;
using System.Windows.Data;
using Wpf.Ui.Controls;

namespace DongKeJi.UI.Converter;

/// <summary>
///     是否登录状态图标转换
/// </summary>
internal class LoggedStateToSymbolConverter : IValueConverter
{
    /// <summary>
    ///     已登录
    /// </summary>
    private static SymbolRegular Logged => SymbolRegular.CheckmarkUnderlineCircle16;

    /// <summary>
    ///     已登出
    /// </summary>
    private static SymbolRegular Logout => SymbolRegular.Empty;


    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is true ? Logged : Logout;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not SymbolRegular symbol) return false;

        return symbol == SymbolRegular.CheckmarkUnderlineCircle16;
    }
}