using System.Windows;
using System.Windows.Controls;
using CountingConsumeViewModel = DongKeJi.Work.ViewModel.Common.Consume.CountingConsumeViewModel;
using MixingConsumeViewModel = DongKeJi.Work.ViewModel.Common.Consume.MixingConsumeViewModel;
using TimingConsumeViewModel = DongKeJi.Work.ViewModel.Common.Consume.TimingConsumeViewModel;

namespace DongKeJi.Work.UI.TemplateSelector;

/// <summary>
///     划扣模板选择器
/// </summary>
internal class ConsumeDataTemplateSelector : DataTemplateSelector
{
    public required DataTemplate Timing { get; set; }
    public required DataTemplate Couting { get; set; }
    public required DataTemplate Mixing { get; set; }


    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        if (item is TimingConsumeViewModel) return Timing;

        if (item is CountingConsumeViewModel) return Couting;

        if (item is MixingConsumeViewModel) return Mixing;

        return base.SelectTemplate(item, container);
    }
}