﻿using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DongKeJi.Work.ViewModel.Common.Consume;

/// <summary>
///     计时划扣
/// </summary>
public partial class ConsumeMixingViewModel : ConsumeViewModel
{
    /// <summary>
    ///     划扣张数
    /// </summary>
    [ObservableProperty]
    [Required(ErrorMessage = "划扣张数不可为空")]
    [Range(0.5, 99999, ErrorMessage = "划扣张数需要>=0.5 且 < 99999")]
    
    private double _consumeCounts;

    /// <summary>
    ///     划扣天数
    /// </summary>
    [ObservableProperty]
    [Required(ErrorMessage = "划扣天数不可为空")]
    [Range(0.5, 99999, ErrorMessage = "划扣天数需要>=0.5 且 < 99999")]
    private double _consumeDays;
}