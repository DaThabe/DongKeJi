using System.Windows;
using System.Windows.Controls;
using DongKeJi.Work.ViewModel.Common.Order;

namespace DongKeJi.Work.UI.TemplateSelector;

/// <summary>
///     订单模板选择器
/// </summary>
internal class OrderDataTemplateSelector : DataTemplateSelector
{
    public required DataTemplate Timing { get; set; }
    public required DataTemplate Counting { get; set; }
    public required DataTemplate Mixing { get; set; }


    public override DataTemplate SelectTemplate(object? item, DependencyObject container)
    {
        if (item is TimingOrderViewModel) return Timing;

        if (item is CountingOrderViewModel) return Counting;

        if (item is MixingOrderViewModel) return Mixing;

        return base.SelectTemplate(item, container);
    }
}