using System.Globalization;
using System.Windows.Controls;

namespace DongKeJi.Common.Validation.Rule;


/// <summary>
/// 自定义验证
/// </summary>
/// <typeparam name="TValue"></typeparam>
/// <param name="validateProcess"></param>
public class LambdaValidationRule<TValue>(Func<TValue, ValidationResult> validateProcess) : ValidationRule<TValue>
{
    public override ValidationResult Validate(TValue value, CultureInfo cultureInfo)
    {
        return validateProcess(value);
    }


    public static implicit operator LambdaValidationRule<TValue>(Func<TValue, ValidationResult> validateProcess)
    {
        return new LambdaValidationRule<TValue>(validateProcess);
    }
}

public class LambdaValidationRule(Func<object, ValidationResult> validateProcess) : LambdaValidationRule<object>(validateProcess);