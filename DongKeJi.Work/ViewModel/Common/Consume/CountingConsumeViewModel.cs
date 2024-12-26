using CommunityToolkit.Mvvm.ComponentModel;

namespace DongKeJi.Work.ViewModel.Common.Consume;

/// <summary>
///     计时划扣
/// </summary>
public partial class CountingConsumeViewModel : ConsumeViewModel
{
    /// <summary>
    ///     划扣张数
    /// </summary>
    [ObservableProperty] private double _consumeCounts;
}