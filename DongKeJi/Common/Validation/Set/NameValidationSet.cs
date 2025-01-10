using DongKeJi.Common.Validation.Rule;

namespace DongKeJi.Common.Validation.Set;


/// <summary>
/// 名称验证集合
/// </summary>
public class NameValidationSet : ValidationSet<string>
{
    /// <summary>
    /// 默认值
    /// </summary>
    public static NameValidationSet Default { get; } = [];




    public IntRangeValidationRule LengthRange { get; init; } = new IntRangeValidationRule
    {
        Minimum = 1,
        Maximum = 32,
        MinimumMessage = "名称长度不可小于1",
        MaximumMessage = "名称长度不可大于32"
    };

    public TextValidationRule Text { get; init; } = new TextValidationRule
    {
        IsControlMessage = "名称不可为特殊字符",
        NullOrWhiteSpaceMessage = "名称不可为空白字符"
    };


    public NameValidationSet()
    {
        Add(x => x.Length, LengthRange);
        Add(Text);
    }
}