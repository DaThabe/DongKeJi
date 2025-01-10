using System.Globalization;
using System.Windows.Controls;

namespace DongKeJi.Common.Validation.Rule;

/// <summary>
/// 名称验证规则, 长度限定范围内不可为转义字符和空白字符
/// </summary>
public class TextValidationRule : ValidationRule<string>
{
    public string NullOrWhiteSpaceMessage { get; set; } = "名称不可为空";

    public string IsControlMessage { get; set; } = "名称不能包含转义字符";


    public override ValidationResult Validate(string value, CultureInfo cultureInfo)
    {
        // 1. 检查是否是非空字符串，且不全是空格
        if (string.IsNullOrWhiteSpace(value))
        {
            return new ValidationResult(false, NullOrWhiteSpaceMessage);
        }

        // 2. 检查字符串中是否包含控制字符
        if (value.Any(char.IsControl))
        {
            return new ValidationResult(false, IsControlMessage);
        }

        // 3.验证名称长度范围
        return ValidationResult.ValidResult;
    }
}