namespace DongKeJi.Common.Exceptions;

/// <summary>
/// 储存库异常类型
/// </summary>
public enum RepositoryExceptionType
{
    /// <summary>
    /// 主键冲突, 通常是实体已存在
    /// </summary>
    PrimaryKeyConflict,

    /// <summary>
    /// 主键缺失, 通常实体不存在
    /// </summary>
    PrimaryKeyMissing,

    /// <summary>
    /// 外键缺失, 通常是关联数据不存在
    /// </summary>
    ForeignKeyMissing,

    /// <summary>
    /// 保存失败
    /// </summary>
    SaveFailed
}