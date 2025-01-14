﻿using CommunityToolkit.Mvvm.ComponentModel;
using DongKeJi.ViewModel;
using DongKeJi.Work.ViewModel.Staff;

namespace DongKeJi.Work.ViewModel.Consume;

/// <summary>
/// 设计师划扣
/// </summary>
/// <param name="designer"></param>
/// <param name="consume"></param>
public partial class ConsumeDesignerViewModel(
    ConsumeViewModel consume,
    StaffViewModel designer
) : ObservableViewModel
{
    public event Action<StaffViewModel>? DesignerChanged;

    /// <summary>
    /// 设计师
    /// </summary>
    [ObservableProperty] private StaffViewModel _designer = designer;
    /// <summary>
    /// 划扣
    /// </summary>
    [ObservableProperty] private ConsumeViewModel _consume = consume;

    partial void OnDesignerChanged(StaffViewModel value)
    {
        DesignerChanged?.Invoke(value);
    }


    public override string ToString()
    {
        return $"{Designer}-{Consume}";
    }
}