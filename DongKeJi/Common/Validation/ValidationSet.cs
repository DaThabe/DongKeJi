using DongKeJi.Common.Exceptions;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Controls;

namespace DongKeJi.Common.Validation;


/// <summary>
/// 规则验证集合
/// </summary>
public class ValidationSet<TValue> : Collection<ValidationRule>, IValidation<TValue>, IEnumerable<ValidationRule<TValue>>
{
    /// <summary>
    /// 添加一个指定类型的规则
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TValidationValue"></typeparam>
    /// <param name="validationValueGetter"></param>
    /// <param name="rule"></param>
    public void Add<TValidationValue>(
        Func<TValue, TValidationValue> validationValueGetter,
        ValidationRule<TValidationValue> rule)
    {
        var validationRule = new LambdaValidationRule<TValue, TValidationValue>(validationValueGetter, rule);
        Add(validationRule);
    }

    /// <summary>
    /// 添加一些指定类型的规则
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TValidationValue"></typeparam>
    /// <param name="validationValueGetter"></param>
    /// <param name="values"></param>
    public void Add<TValidationValue>(
        Func<TValue, TValidationValue> validationValueGetter,
        IEnumerable<ValidationRule<TValidationValue>> values)
    {
        foreach (var rule in values)
        {
            var validationRule = new LambdaValidationRule<TValue, TValidationValue>(validationValueGetter, rule);
            Add(validationRule);
        }
    }

    /// <summary>
    /// 添加一些指定类型的规则
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TValidationValue"></typeparam>
    /// <param name="validationValueGetter"></param>
    /// <param name="values"></param>
    public void Add<TValidationValue>(
        Func<TValue, TValidationValue> validationValueGetter,
        ValidationSet<TValidationValue> values)
    {
        foreach (var rule in values)
        {
            var validationRule = new LambdaValidationRule<TValue, TValidationValue>(validationValueGetter, rule);
            Add(validationRule);
        }
    }

    

    public new IEnumerator<ValidationRule<TValue>> GetEnumerator()
    {
        return this.OfType<ValidationRule<TValue>>().GetEnumerator();
    }

    public System.ComponentModel.DataAnnotations.ValidationResult Validate(TValue value)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// 
/// </summary>
public class ValidationSet : ValidationSet<object>;


public static class ValidationSetExtensions
{
    public static ValidationResult Validate<TValue>(this IEnumerable<ValidationRule<TValue>> ruleSet, TValue value)
    {
        try
        {
            foreach (var rule in ruleSet)
            {
                var result = rule.Validate(value, CultureInfo.CurrentCulture);
                if (!result.IsValid) return result;
            }

            return ValidationResult.ValidResult;
        }
        catch (Exception e)
        {
            return new ValidationResult(false, e.FormatAllMessage());
        }
    }

    public static ValidationResult Validate(this IEnumerable<ValidationRule> ruleSet, object value)
    {
        try
        {
            foreach (var rule in ruleSet)
            {
                var result = rule.Validate(value, CultureInfo.CurrentCulture);
                if (!result.IsValid) return result;
            }

            return ValidationResult.ValidResult;
        }
        catch (Exception e)
        {
            return new ValidationResult(false, e.FormatAllMessage());
        }
    }
}

file class LambdaValidationRule<TValue, TValidationValue>(
    Func<TValue, TValidationValue> validationValueGetter,
    ValidationRule<TValidationValue> validationRule) : ValidationRule<TValue>
{
    public override ValidationResult Validate(TValue value, CultureInfo cultureInfo)
    {
        var validationValue = validationValueGetter(value);

        if (validationValue is null)
        {
            return new ValidationResult(false, $"数据筛选失败, 筛选结果为空, {typeof(TValue).Name}->{typeof(TValidationValue).Name}");
        }

        return validationRule.Validate(validationValue);
    }
}