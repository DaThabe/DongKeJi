namespace DongKeJi.Work.ViewModel.Common.Customer;

/// <summary>
/// 表示一个可以为空的对象
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IEmptyable<out T>
{
    bool IsEmpty { get; }
    static abstract T Empty { get; }
}