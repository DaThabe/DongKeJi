using CommunityToolkit.Mvvm.ComponentModel;
using DongKeJi.Work.ViewModel.Staff;
using DongKeJi.ViewModel;
using System.Collections.ObjectModel;

namespace DongKeJi.Work.ViewModel.Order;

public partial class OrderEditViewModel : ObservableViewModel
{
    /// <summary>
    /// 创建
    /// </summary>
    /// <param name="salespersons"></param>
    /// <returns></returns>
    public static OrderEditViewModel Create(ObservableCollection<StaffViewModel> salespersons)
    {
        return new OrderEditViewModel
        {
            SalespersonSelector = new StaffSelectorViewModel()
            {
                ItemsSource = salespersons,
                Selected = salespersons.FirstOrDefault()
            }
        };
    }


    /// <summary>
    /// 销售选择器
    /// </summary>
    [ObservableProperty]
    private StaffSelectorViewModel _salespersonSelector = new StaffSelectorViewModel();

    /// <summary>
    /// 订单信息
    /// </summary>
    [ObservableProperty] private OrderViewModel _order = new OrderTimingViewModel();
}