namespace DongKeJi;

/// <summary>
///     表示一个包含默认值的对象
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IDefault<out T>
{
    /// <summary>
    ///     当前实例是否是默认值
    /// </summary>
    bool IsDefault { get; }

    /// <summary>
    ///     当前类型的默认值
    /// </summary>
    static abstract T Default { get; }
}