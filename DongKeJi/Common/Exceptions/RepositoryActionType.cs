namespace DongKeJi.Common.Exceptions;

/// <summary>
/// 储存库行为类型
/// </summary>
public enum RepositoryActionType
{
    /// <summary>
    /// 获取
    /// </summary>
    Get,

    /// <summary>
    /// 添加
    /// </summary>
    Add,

    /// <summary>
    /// 查找
    /// </summary>
    Find,

    /// <summary>
    /// 删除
    /// </summary>
    Remove,

    /// <summary>
    /// 更新
    /// </summary>
    Update
}