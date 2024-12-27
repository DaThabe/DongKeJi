namespace DongKeJi.Common;

/// <summary>
///     表示一个可以为空的对象
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IEmptyable<out T>
{
    /// <summary>
    ///     当前实例是否是空元素
    /// </summary>
    bool IsEmpty { get; }

    /// <summary>
    ///     当前类型空元素实例
    /// </summary>
    static abstract T Empty { get; }
}