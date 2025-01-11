using System.ComponentModel.DataAnnotations;

namespace DongKeJi.Validation;

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


    public static void AssertValidate(bool isError, string errorMessage, params string[] memberNames)
    {
        if (isError == false) return;

        ValidationResult result = new(errorMessage, memberNames);
        throw new ValidationException(result, null, null);
    }

    public static void AssertValidate<T>(T value, bool isError, string errorMessage, params string[] memberNames)
    {
        if (isError == false) return;

        ValidationResult result = new(errorMessage, memberNames);
        throw new ValidationException(result, null, value);
    }
}