using System.Globalization;
using System.Windows.Controls;

namespace DongKeJi.Common.Validation;


/// <summary>
/// 数据验证条件
/// </summary>
public abstract class ValidationRule<TValue> : ValidationRule, IValidation<TValue>
{
    public abstract ValidationResult Validate(TValue value, CultureInfo cultureInfo);


    public ValidationResult Validate(TValue value)
    {
        return Validate(value, CultureInfo.CurrentCulture);
    }

    public override ValidationResult Validate(object? value, CultureInfo cultureInfo)
    {
        if (value is not TValue data)
        {
            return new ValidationResult(false, $"数据为空或不匹配, 需为: {typeof(TValue).Name}实例");
        }

        return Validate(data, cultureInfo);
    }
}