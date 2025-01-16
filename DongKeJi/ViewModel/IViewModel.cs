using System.ComponentModel;

namespace DongKeJi.ViewModel;

/// <summary>
///     视图模型接口
/// </summary>
public interface IViewModel : INotifyPropertyChanged, INotifyPropertyChanging;

/// <summary>
/// 扩展方法
/// </summary>
public static class ViewModelExtensions
{
    /// <summary>
    /// 某个属性改变后的执行过程
    /// </summary>
    /// <param name="viewModel"></param>
    /// <param name="propertyName"></param>
    /// <param name="action"></param>
    /// <param name="exception"></param>
    /// <returns></returns>
    public static IPropertyChangHandler PropertyChangedAction(
        this INotifyPropertyChanged viewModel,
        string propertyName,
        Action action,
        Action<Exception>? exception)
    {
        return new PropertyChangedHandler(viewModel, (sender, e) =>
        {
            if (e.PropertyName != propertyName) return;

            try
            {
                action();
            }
            catch (Exception ex)
            {
                exception?.Invoke(ex);
            }
        });
    }

    /// <summary>
    /// 某个属性改变中的执行过程
    /// </summary>
    /// <param name="viewModel"></param>
    /// <param name="propertyName"></param>
    /// <param name="action"></param>
    /// <param name="exception"></param>
    /// <returns></returns>
    public static IPropertyChangHandler PropertyChangingAction(
        this INotifyPropertyChanging viewModel,
        string propertyName,
        Action action,
        Action<Exception>? exception)
    {
        return new PropertyChangingHandler(viewModel, (sender, e) =>
        {
            if (e.PropertyName != propertyName) return;

            try
            {
                action();
            }
            catch (Exception ex)
            {
                exception?.Invoke(ex);
            }
        });
    }
}


/// <summary>
/// 属性改变句柄
/// </summary>
public interface IPropertyChangHandler : IDisposable;

/// <summary>
/// 属性改变后句柄
/// </summary>
public class PropertyChangedHandler : IPropertyChangHandler
{
    private readonly INotifyPropertyChanged _event;

    private readonly PropertyChangedEventHandler _action;

    public PropertyChangedHandler(INotifyPropertyChanged @event, PropertyChangedEventHandler action)
    {
        _event = @event;
        _action = action;

        _event.PropertyChanged += action;
    }

    public void Dispose()
    {
        _event.PropertyChanged -= _action;
        GC.SuppressFinalize(this);
    }
}

/// <summary>
/// 属性改变前句柄
/// </summary>
public class PropertyChangingHandler : IPropertyChangHandler
{
    private readonly INotifyPropertyChanging _event;

    private readonly PropertyChangingEventHandler _action;

    public PropertyChangingHandler(INotifyPropertyChanging @event, PropertyChangingEventHandler action)
    {
        _event = @event;
        _action = action;

        _event.PropertyChanging += action;
    }

    public void Dispose()
    {
        _event.PropertyChanging -= _action;
        GC.SuppressFinalize(this);
    }
}