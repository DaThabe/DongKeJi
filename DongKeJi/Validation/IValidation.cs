using System.ComponentModel.DataAnnotations;

namespace DongKeJi.Validation;

/// <summary>
/// 数据验证
/// </summary>
public interface IValidation
{
    /// <summary>
    /// 所有错误
    /// </summary>
    IEnumerable<ValidationResult> Errors { get; }

    /// <summary>
    /// 是否有错误
    /// </summary>
    bool HasErrors { get; }

    /// <summary>
    /// 验证
    /// </summary>
    void Validate();
}