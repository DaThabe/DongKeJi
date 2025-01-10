using System.Numerics;

namespace DongKeJi.Common.Validation.Rule;

/// <summary>
/// 范围验证规则参数
/// </summary>
/// <typeparam name="TNumber"></typeparam>
public interface IRangeValidationRuleParam<TNumber>  where TNumber : INumber<TNumber>
{
    /// <summary>
    /// 最小值
    /// </summary>
    TNumber Minimum { get; set; }

    /// <summary>
    /// 最大值
    /// </summary>
    TNumber Maximum { get; set; }

    /// <summary>
    /// 小于消息
    /// </summary>
    string MinimumMessage { get; set; }

    /// <summary>
    /// 大于消息
    /// </summary>
    string MaximumMessage { get; set; }
}