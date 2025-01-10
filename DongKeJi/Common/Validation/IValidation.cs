using System.ComponentModel.DataAnnotations;

namespace DongKeJi.Common.Validation;

/// <summary>
/// 数据验证接口
/// </summary>
/// <typeparam name="TValue"></typeparam>
public interface IValidation<in TValue>
{
    ValidationResult Validate(TValue value);
}

/// <summary>
/// 数据验证
/// </summary>
public interface IValidation
{
    IEnumerable<ValidationResult> Validate();
}

public static class ValidationExtensions
{
    /// <summary>
    /// 断言验证
    /// </summary>
    /// <param name="validation"></param>
    /// <exception cref="ValidationException"></exception>
    public static void AssertValidate(this IValidation validation)
    {
        var resultList = validation.Validate();

        foreach (var i in resultList)
        {
            if (i == ValidationResult.Success) continue;
            throw new ValidationException(i, null, validation);
        }
    }

    /// <summary>
    /// 断言验证
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="validation"></param>
    /// <param name="value"></param>
    /// <exception cref="ValidationException"></exception>
    public static void AssertValidate<TValue>(this IValidation<TValue> validation, TValue value)
    {
        var result = validation.Validate(value);

        if (result != ValidationResult.Success)
        {
            throw new ValidationException(result.ErrorMessage);
        }
    }
}