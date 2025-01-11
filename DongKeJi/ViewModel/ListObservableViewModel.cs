using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace DongKeJi.ViewModel;

/// <summary>
///     列表VM
/// </summary>
[Obsolete("直接用通知集合类: ObservableCollection")]
public partial class ListObservableViewModel<TViewModel> : ObservableViewModel
{
    /// <summary>
    ///     是否是空的
    /// </summary>
    [ObservableProperty] private bool _isEmpty = true;


    /// <summary>
    ///     所有元素
    /// </summary>
    [ObservableProperty] private ObservableCollection<TViewModel> _items;

    /// <summary>
    ///     当前选中的
    /// </summary>
    [ObservableProperty] private TViewModel? _selected;


    /// <inheritdoc />
    public ListObservableViewModel() : this([])
    {
    }

    /// <inheritdoc />
    public ListObservableViewModel(params TViewModel[] array) : this(values: array)
    {
    }

    /// <inheritdoc />
    public ListObservableViewModel(IEnumerable<TViewModel> values)
    {
        _items = new ObservableCollection<TViewModel>(values);
        Selected = Enumerable.FirstOrDefault<TViewModel>(Items);
    }

    /// <summary>
    ///     选中了新的元素
    /// </summary>
    public event Action<TViewModel?>? SelectedChanged;

    public void Clear()
    {
        Items.Clear();
        Selected = default;
        IsEmpty = true;
    }


    partial void OnItemsChanged(ObservableCollection<TViewModel> value)
    {
        IsEmpty = value.Count <= 0;
        Selected = value.FirstOrDefault();

        value.CollectionChanged += (sender, e) => { IsEmpty = Items.Count <= 0; };
    }

    partial void OnSelectedChanged(TViewModel? value)
    {
        SelectedChanged?.Invoke(value);
    }
}