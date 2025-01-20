using CommunityToolkit.Mvvm.ComponentModel;
using DongKeJi.ViewModel;
using System.Collections.ObjectModel;

namespace DongKeJi.Work.ViewModel.Staff;

public partial class StaffSelectorViewModel : ObservableViewModel
{
    /// <summary>
    /// 选中的
    /// </summary>
    [ObservableProperty]
    private StaffViewModel? _selected;

    /// <summary>
    /// 可选的
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<StaffViewModel> _itemsSource = [];
}