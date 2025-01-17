namespace DongKeJi.Deploy.Model;

/// <summary>
/// 更新类型
/// </summary>
public enum UpdateTypeEnum
{
    /// <summary>
    /// 修复
    /// </summary>
    Patch,

    /// <summary>
    /// 功能增强
    /// </summary>
    Minor,

    /// <summary>
    /// 大版本更新
    /// </summary>
    Major
}