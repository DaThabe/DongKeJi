namespace DongKeJi.Common.Validation.Rule;

/// <summary>
/// 文本验证参数
/// </summary>
public interface ITextValidationRuleParam : IRangeValidationRuleParam<int>
{
    /// <summary>
    /// 名称空白提示
    /// </summary>
    string NullOrWhiteSpaceMessage { get; set; }

    /// <summary>
    /// 名字控制字符提示
    /// </summary>
    string IsControlMessage { get; set; }
}