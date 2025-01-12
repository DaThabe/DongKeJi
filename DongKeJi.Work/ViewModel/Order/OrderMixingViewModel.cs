﻿using CommunityToolkit.Mvvm.ComponentModel;
using DongKeJi.Work.Model.Entity.Order;
using System.ComponentModel.DataAnnotations;

namespace DongKeJi.Work.ViewModel.Order;

/// <summary>
///     计数订阅订单
/// </summary>
public partial class OrderMixingViewModel : OrderViewModel
{
    public override OrderType Type => OrderType.Mixing;

    /// <summary>
    ///     初始张数
    /// </summary>
    [ObservableProperty]
    [Required(ErrorMessage = "初始张数不可为空")]
    [Range(0, 99999, ErrorMessage = "初始天数需要>=0 且 < 99999")] 
    private double _initCounts;

    /// <summary>
    ///     总张数
    /// </summary>
    [ObservableProperty]
    [Required(ErrorMessage = "总张数不可为空")]
    [Range(0.5, 99999, ErrorMessage = "总天数需要>=0.5 且 < 99999")]
    private double _totalCounts;

    /// <summary>
    ///     初始天数
    /// </summary>
    [ObservableProperty]
    [Required(ErrorMessage = "初始天数不可为空")]
    [Range(0, 99999, ErrorMessage = "初始张数需要>=0 且 < 99999")] 
    private double _initDays;

    /// <summary>
    ///     总天数
    /// </summary>
    [ObservableProperty]
    [Required(ErrorMessage = "总天数不可为空")]
    [Range(0.5, 99999, ErrorMessage = "总天数需要>=0.5 且 < 99999")] 
    private double _totalDays;



    partial void OnInitCountsChanging(double value) => ValidateProperty(value, nameof(InitCounts));

    partial void OnTotalCountsChanging(double value) => ValidateProperty(value, nameof(TotalCounts));

    partial void OnInitDaysChanging(double value) => ValidateProperty(value, nameof(InitDays));

    partial void OnTotalDaysChanging(double value) => ValidateProperty(value, nameof(TotalDays));
}