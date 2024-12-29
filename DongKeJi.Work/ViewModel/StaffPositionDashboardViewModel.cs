using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DongKeJi.Common.Extensions;
using DongKeJi.Common.Inject;
using DongKeJi.Common.UI;
using DongKeJi.Common.ViewModel;
using DongKeJi.ViewModel.User;
using DongKeJi.Work.Model.Entity.Staff;
using DongKeJi.Work.Service;
using DongKeJi.Work.UI.View.Common.Staff;
using DongKeJi.Work.ViewModel.Common.Staff;
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
public partial class StaffPositionDashboardViewModel(
    ILogger<StaffDashboardViewModel> logger,
    ISnackbarService snackbarService,
    IContentDialogService contentDialogService,
    IStaffRepository staffRepository,
    IStaffPositionService staffPositionService
) : LazyInitializeViewModel, IStaffPositionDashboardContext
{
    #region --上下文属性--

    /// <summary>
    ///     员工
    /// </summary>
    [ObservableProperty] private StaffViewModel _staff  = StaffViewModel.Empty;

    /// <summary>
    ///     员工列表
    /// </summary>
    [ObservableProperty] private ObservableCollection<StaffViewModel> _staffList = [];


    /// <summary>
    ///     职位
    /// </summary>
    [ObservableProperty] private StaffPositionViewModel _position = StaffPositionViewModel.Empty;

    /// <summary>
    ///     职位列表
    /// </summary>
    [ObservableProperty] private ObservableCollection<StaffPositionViewModel> _positionList = [];

    #endregion

    #region --默认行为&初始化--

    protected override async Task OnInitializationAsync(CancellationToken cancellation = default)
    {
        await ReloadPositionCommand.ExecuteAsync(null);
    }

    partial void OnPositionChanged(StaffPositionViewModel? value)
    {
        Position = value ?? StaffPositionViewModel.Empty;

        Staff = StaffViewModel.Empty;
        StaffList.Clear();

        if (Position != StaffPositionViewModel.Empty)
        {
            ReloadStaffCommand.ExecuteAsync(null);
        }
    }

    #endregion

    #region --职位--

    /// <summary>
    ///     创建职位
    /// </summary>
    /// <returns></returns>
    private async ValueTask<StaffPositionViewModel?> CreatePositionAsync()
    {
        var positionCreatorViewModel = new StaffPositionCreatorViewModel();

        var content = new SimpleContentDialogCreateOptions
        {
            Title = "新增职位",
            Content = new StaffPositionCreatorView { DataContext = positionCreatorViewModel },
            PrimaryButtonText = "创建",
            CloseButtonText = "取消"
        };

        //弹窗

        while (true)
        {
            var dialogResult = await contentDialogService.ShowSimpleDialogAsync(content);

            if (dialogResult != ContentDialogResult.Primary) return null;

            if (string.IsNullOrWhiteSpace(positionCreatorViewModel.Position.Title))
            {
                snackbarService.ShowWarning("标题不可为空");
                continue;
            }

            break;
        }

        //等待确认
        return positionCreatorViewModel.Position;
    }

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

            PositionList = positions.ToObservableCollection();
            Position = PositionList.FirstOrDefault() ?? StaffPositionViewModel.Empty;
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
            var index = PositionList.RemoveAtMatchedIndex(x => x.Type == position.Type);
            Position = PositionList.TryGetElementWithOffset(index, -1) ?? StaffPositionViewModel.Empty;

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
    /// <param name="name"></param>
    /// <returns></returns>
    [RelayCommand]
    private async Task AddPositionAsync()
    {
        var position = await CreatePositionAsync();
        if (position is null) return;


        try
        {
            await staffPositionService.SetAsync(position);
            PositionList.Add(position, x => x.Type != position.Type);
            Position = position;
        }
        catch (Exception ex)
        {
            snackbarService.ShowError(ex);
            logger.LogError(ex, "职位添加失败, 类型:{type}", position);
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
            var staffs = await staffRepository.FindAllByPositionTypeAsync(Position.Type);

            StaffList = staffs.ToObservableCollection();
            Staff = StaffList.FirstOrDefault() ?? StaffViewModel.Empty;
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
            await staffPositionService.UnbindingAsync(Position.Type, Staff);

            //删除
            var index = StaffList.RemoveAtMatchedIndex(x => x.Id == staff.Id);
            Staff = StaffList.TryGetElementWithOffset(index, -1) ?? StaffViewModel.Empty;

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