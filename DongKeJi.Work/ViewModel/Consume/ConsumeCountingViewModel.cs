using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;

namespace DongKeJi.Work.ViewModel.Consume;

/// <summary>
///     计时划扣
/// </summary>
public partial class ConsumeCountingViewModel : ConsumeViewModel
{
    /// <summary>
    ///     划扣张数
    /// </summary>
    [ObservableProperty]
    [Required(ErrorMessage = "划扣张数不可为空")]
    [Range(0.5, 99999, ErrorMessage = "划扣天数需要>=0.5 且 < 99999")]
    private double _consumeCounts;


    partial void OnConsumeCountsChanged(double value) => ValidateProperty(value, nameof(ConsumeCounts));
}