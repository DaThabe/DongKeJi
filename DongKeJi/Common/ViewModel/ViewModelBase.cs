using CommunityToolkit.Mvvm.ComponentModel;

namespace DongKeJi.Common.ViewModel;

/// <summary>
/// 视图模型基类
/// </summary>
public abstract class ViewModelBase : ObservableObject, IViewModel
{
    public override string ToString()
    {
        return GetType().Name;
    }
}