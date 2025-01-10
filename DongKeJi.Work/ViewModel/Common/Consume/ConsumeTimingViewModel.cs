using CommunityToolkit.Mvvm.ComponentModel;

namespace DongKeJi.Work.ViewModel.Common.Consume;

/// <summary>
///     计时划扣
/// </summary>
public partial class ConsumeTimingViewModel : ConsumeViewModel
{
    /// <summary>
    ///     划扣天数
    /// </summary>
    [ObservableProperty] private double _consumeDays;
}