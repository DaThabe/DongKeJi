using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;

namespace DongKeJi.ViewModel;

/// <summary>
///     视图模型接口
/// </summary>
public interface IViewModel : INotifyPropertyChanged, INotifyPropertyChanging;

/// <summary>
/// 视图模型
/// </summary>
public abstract class ObservableViewModel : ObservableObject, IViewModel;