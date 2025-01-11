using System.ComponentModel.DataAnnotations;

namespace DongKeJi.Validation;

/// <summary>
/// 数据验证
/// </summary>
public interface IValidation
{
    IEnumerable<ValidationResult> Validate();
}