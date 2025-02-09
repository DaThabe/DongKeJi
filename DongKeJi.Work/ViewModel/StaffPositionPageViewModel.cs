﻿using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DongKeJi.Extensions;
using DongKeJi.Inject;
using DongKeJi.UI;
using DongKeJi.ViewModel;
using DongKeJi.Work.Service;
using DongKeJi.Work.UI.View.Staff;
using DongKeJi.Work.ViewModel.Staff;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Wpf.Ui;
using Wpf.Ui.Controls;
using Wpf.Ui.Extensions;

namespace DongKeJi.Work.ViewModel;

/// <summary>
///     员工职位管理
/// </summary>
[Inject(ServiceLifetime.Transient)]
public partial class StaffPositionPageViewModel(
    ILogger<StaffPageViewModel> logger,
    ISnackbarService snackbarService,
    IContentDialogService contentDialogService,
    IWorkDatabase dbService,
    IStaffService staffService,
    IStaffPositionService staffPositionService
) : LazyInitializeViewModel
{
    #region --上下文属性--

    /// <summary>
    ///     员工
    /// </summary>
    [ObservableProperty] private StaffViewModel? _selectedStaff;

    /// <summary>
    ///     员工列表
    /// </summary>
    [ObservableProperty] private ObservableCollection<StaffViewModel> _staffCollection = [];


    /// <summary>
    ///     职位
    /// </summary>
    [ObservableProperty] private StaffPositionViewModel? _selectedPosition;

    /// <summary>
    ///     职位列表
    /// </summary>
    [ObservableProperty] private ObservableCollection<StaffPositionViewModel> _positionCollection = [];

    #endregion

    #region --默认行为&初始化--

    protected override async ValueTask OnInitializationAsync(CancellationToken cancellation = default)
    {
        await ReloadPositionCommand.ExecuteAsync(null);
    }

    partial void OnSelectedPositionChanged(StaffPositionViewModel? value)
    {
        if (SelectedPosition is null) return;
        ReloadStaffCommand.ExecuteAsync(null);
    }

    #endregion

    #region --职位--

    /// <summary>
    ///     刷新
    /// </summary>
    /// <returns></returns>
    [RelayCommand]
    private async Task ReloadPositionAsync()
    {
        try
        {
            var positions = await staffPositionService.GetAllAsync();

            PositionCollection = positions.ToObservableCollection();
            PositionCollection.ForEach(x => dbService.AutoUpdate(x));
            SelectedPosition = PositionCollection.FirstOrDefault();
        }
        catch (Exception ex)
        {
            snackbarService.ShowError(ex);
            logger.LogError(ex, "加载职位时发生错误");
        }
    }

    /// <summary>
    ///     删除
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    [RelayCommand]
    private async Task RemovePositionAsync(StaffPositionViewModel position)
    {
        try
        {
            SimpleContentDialogCreateOptions dialog = new()
            {
                Title = "是否删除职位信息",
                Content = $"职位类型:{position.Type}, 职位名称:{position.Title}",
                PrimaryButtonText = "确认",
                CloseButtonText = "取消"
            };

            //弹窗
            var dialogResult = await contentDialogService.ShowSimpleDialogAsync(dialog);

            //等待确认
            if (dialogResult != ContentDialogResult.Primary) return;

            //更新数据库
            await staffPositionService.RemoveAsync(position.Type);

            //删除
            var index = PositionCollection.RemoveAtMatchedIndex(x => x.Type == position.Type);
            SelectedPosition = PositionCollection.TryGetElementWithOffset(index, -1);

            snackbarService.ShowSuccess($"已删除职位, 类型:{position.Type},标题:{position.Title}");
        }
        catch (Exception ex)
        {
            snackbarService.ShowError(ex);
            logger.LogError(ex, "删除职位失败, 类型:{type},标题:{title}", position.Type, position.Title);
        }
    }

    /// <summary>
    ///     添加
    /// </summary>
    /// <returns></returns>
    [RelayCommand]
    private async Task AddPositionAsync()
    {
        try
        {
            var creatorVm = new StaffPositionCreatorViewModel();

            var content = new SimpleContentDialogCreateOptions
            {
                Title = "新增职位",
                Content = new StaffPositionCreatorView { DataContext = creatorVm },
                PrimaryButtonText = "创建",
                CloseButtonText = "取消"
            };

            var dialogResult = await contentDialogService.ShowSimpleDialogAsync(content);
            if (dialogResult != ContentDialogResult.Primary) return;


            await staffPositionService.SetAsync(creatorVm.Position);
            PositionCollection.Add(creatorVm.Position, x => x.Type != creatorVm.Position.Type);
            dbService.AutoUpdate(creatorVm.Position);
            SelectedPosition = creatorVm.Position;
        }
        catch (Exception ex)
        {
            snackbarService.ShowError(ex);
            logger.LogError(ex, "职位添加失败");
        }
    }

    #endregion

    #region --员工--

    /// <summary>
    ///     刷新员工
    /// </summary>
    /// <returns></returns>
    [RelayCommand]
    private async Task ReloadStaffAsync()
    {
        try
        {
            ArgumentNullException.ThrowIfNull(SelectedPosition);

            var staffs = await staffService.GetAllByPositionTypeAsync(SelectedPosition.Type);

            StaffCollection = staffs.ToObservableCollection();
            StaffCollection.ForEach(x => dbService.AutoUpdate(x));
            SelectedStaff = StaffCollection.FirstOrDefault();
        }
        catch (Exception ex)
        {
            snackbarService.ShowError(ex);
            logger.LogError(ex, "加载员工时发生错误");
        }
    }


    /// <summary>
    ///     取消绑定员工
    /// </summary>
    /// <param name="staff"></param>
    /// <returns></returns>
    [RelayCommand]
    private async Task UnbindingStaffAsync(StaffViewModel staff)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(SelectedPosition);
            ArgumentNullException.ThrowIfNull(SelectedStaff);

            SimpleContentDialogCreateOptions dialog = new()
            {
                Title = "是否移除员工",
                Content = $"员工名称:{staff.Name}",
                PrimaryButtonText = "确认",
                CloseButtonText = "取消"
            };

            //弹窗
            var dialogResult = await contentDialogService.ShowSimpleDialogAsync(dialog);

            //等待确认
            if (dialogResult != ContentDialogResult.Primary) return;

            //更新数据库
            await staffPositionService.RemoveStaffAsync(SelectedPosition.Type, SelectedStaff);

            //删除
            var index = StaffCollection.RemoveAtMatchedIndex(x => x.Id == staff.Id);
            SelectedStaff = StaffCollection.TryGetElementWithOffset(index, -1);

            snackbarService.ShowSuccess($"已移除员工, 名称:{staff.Name}");
        }
        catch (Exception ex)
        {
            snackbarService.ShowError(ex);
            logger.LogError(ex, "移除员工失败, 名称:{name}", staff.Name);
        }
    }


    #endregion
}