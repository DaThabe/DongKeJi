using System.ComponentModel;

namespace DongKeJi.Common.ViewModel;

/// <summary>
///     视图模型接口
/// </summary>
public interface IViewModel :
    INotifyPropertyChanged,
    INotifyPropertyChanging;


[Obsolete]
public static class ViewModelExtensions
{
    /// <summary>
    ///     注册回调, 仅匹配指定名称
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    /// <param name="vm"></param>
    /// <param name="propertyName"></param>
    /// <param name="callback"></param>
    public static TViewModel PropertyChangedCallback<TViewModel>(this TViewModel vm,
        string propertyName,
        Action callback)
        where TViewModel : IViewModel
    {
        return PropertyChangedCallback(vm, x => x.PropertyName != propertyName, callback);
    }

    /// <summary>
    ///     注册回调, 仅匹配指定名称
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    /// <param name="vm"></param>
    /// <param name="propertyName"></param>
    /// <param name="callback"></param>
    public static TViewModel PropertyChangedCallback<TViewModel>(this TViewModel vm,
        string propertyName,
        Action<TViewModel> callback)
        where TViewModel : IViewModel
    {
        return PropertyChangedCallback(vm, x => x.PropertyName != propertyName, callback);
    }


    /// <summary>
    ///     注册回调, 自定义过滤器
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    /// <param name="vm"></param>
    /// <param name="filter"></param>
    /// <param name="callback"></param>
    public static TViewModel PropertyChangedCallback<TViewModel>(this TViewModel vm,
        Func<PropertyChangedEventArgs, bool> filter,
        Action callback)
        where TViewModel : IViewModel
    {
        vm.PropertyChanged += (_, e) =>
        {
            if (filter(e)) return;
            callback.Invoke();
        };

        return vm;
    }

    /// <summary>
    ///     注册回调, 自定义过滤器
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    /// <param name="vm"></param>
    /// <param name="filter"></param>
    /// <param name="callback"></param>
    public static TViewModel PropertyChangedCallback<TViewModel>(this TViewModel vm,
        Func<PropertyChangedEventArgs, bool> filter,
        Action<TViewModel> callback)
        where TViewModel : IViewModel
    {
        vm.PropertyChanged += (_, e) =>
        {
            if (filter(e)) return;
            callback.Invoke(vm);
        };

        return vm;
    }


    /// <summary>
    ///     注册回调
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    /// <param name="vm"></param>
    /// <param name="callback"></param>
    public static TViewModel PropertyChangedCallback<TViewModel>(this TViewModel vm,
        Action<TViewModel, PropertyChangedEventArgs> callback)
        where TViewModel : IViewModel
    {
        vm.PropertyChanged += (_, e) => { callback.Invoke(vm, e); };

        return vm;
    }

    /// <summary>
    ///     注册回调
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    /// <param name="vm"></param>
    /// <param name="callback"></param>
    public static TViewModel PropertyChangedCallback<TViewModel>(this TViewModel vm,
        Action<TViewModel> callback)
        where TViewModel : IViewModel
    {
        vm.PropertyChanged += (_, _) => { callback.Invoke(vm); };

        return vm;
    }
}