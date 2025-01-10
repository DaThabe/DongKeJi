using System.Globalization;
using System.Numerics;
using System.Windows.Controls;

namespace DongKeJi.Common.Validation.Rule;

/// <summary>
/// 字符串长度验证规则
/// </summary>
public class RangeValidationRule<TNumber> : ValidationRule<TNumber>, IRangeValidationRuleParam<TNumber> 
    where TNumber : INumber<TNumber>
{
    /// <summary>
    /// 最小值
    /// </summary>
    public required TNumber Minimum { get; set; }

    /// <summary>
    /// 最大值
    /// </summary>
    public required TNumber Maximum { get; set; }

    /// <summary>
    /// 小于消息
    /// </summary>
    public string MinimumMessage { get; set; } = "小于范围<{0}";

    /// <summary>
    /// 大于消息
    /// </summary>
    public string MaximumMessage { get; set; } = "大于范围>{0}";


    public override ValidationResult Validate(TNumber value, CultureInfo cultureInfo)
    {
        if (value < Minimum)
        {
            return new ValidationResult(false, string.Format(MinimumMessage, Minimum));
        }

        if (value > Maximum)
        {
            return new ValidationResult(false, string.Format(MinimumMessage, Maximum));
        }

        return ValidationResult.ValidResult;
    }
}

/// <summary>
/// 整形范围验证
/// </summary>
public class IntRangeValidationRule : RangeValidationRule<int>;

/// <summary>
/// 双精度浮点范围验证
/// </summary>
public class DoubleRangeValidationRule : RangeValidationRule<double>;