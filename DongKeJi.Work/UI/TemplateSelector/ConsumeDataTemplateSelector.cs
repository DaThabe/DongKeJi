using System.Windows;
using System.Windows.Controls;
using DongKeJi.Work.ViewModel.Consume;

namespace DongKeJi.Work.UI.TemplateSelector;

/// <summary>
///     划扣模板选择器
/// </summary>
internal class ConsumeDataTemplateSelector : DataTemplateSelector
{
    public required DataTemplate Timing { get; set; }
    public required DataTemplate Counting { get; set; }
    public required DataTemplate Mixing { get; set; }


    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        if (item is ConsumeTimingViewModel) return Timing;

        if (item is ConsumeCountingViewModel) return Counting;

        if (item is ConsumeMixingViewModel) return Mixing;

        return base.SelectTemplate(item, container);
    }
}