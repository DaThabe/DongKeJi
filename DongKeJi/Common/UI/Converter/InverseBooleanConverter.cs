﻿using System.Globalization;
using System.Windows.Data;

namespace DongKeJi.Common.UI.Converter;


/// <summary>
/// 反转布尔结果
/// </summary>
public class InverseBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool b)
        {
            return !b;
        }

        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool b)
        {
            return !b;
        }

        return false;
    }
}