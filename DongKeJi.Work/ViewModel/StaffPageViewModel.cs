using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DongKeJi.Core;
using DongKeJi.Core.ViewModel.User;
using DongKeJi.Extensions;
using DongKeJi.Inject;
using DongKeJi.UI;
using DongKeJi.ViewModel;
using DongKeJi.Work.Model.Entity.Staff;
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
///     员工管理
/// </summary>
[Inject(ServiceLifetime.Transient)]
public partial class StaffPageViewModel(
    ILogger<StaffPageViewModel> logger,
    ISnackbarService snackbarService,
    IContentDialogService contentDialogService,
    IWorkDatabase dbService,
    ICoreModule coreModule,
    IWorkModule workModule,
    IStaffService staffService,
    IStaffPositionService staffPositionService
) : LazyInitializeViewModel
{
    #region --上下文属性--

    public UserViewModel CurrentUser => coreModule.CurrentUser ?? throw new ArgumentNullException(nameof(CurrentUser), "当前用户实例为空");

    public StaffViewModel CurrentStaff => workModule.CurrentStaff ?? throw new ArgumentNullException(nameof(CurrentUser), "当前员工实例为空");


    /// <summary>
    ///     当前选中员工
    /// </summary>
    [ObservableProperty] private StaffViewModel? _selectedStaff;

    /// <summary>
    ///     员工列表
    /// </summary>
    [ObservableProperty] private ObservableCollection<StaffViewModel> _staffCollection = [];

    /// <summary>
    ///     当前选中职位
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
        await ReloadStaffCommand.ExecuteAsync(null);
    }

    partial void OnSelectedStaffChanged(StaffViewModel? value)
    {
        if (SelectedStaff is null) return;
        ReloadPositionCommand.ExecuteAsync(null);
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
            var result = await staffService.GetAllByUserAsync(CurrentUser);

            StaffCollection = result.ToObservableCollection();
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
    ///     删除员工
    /// </summary>
    /// <param name="userStaff"></param>
    /// <returns></returns>
    [RelayCommand]
    private async Task RemoveStaffAsync(StaffViewModel userStaff)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(SelectedStaff);

            if (SelectedStaff.Id == CurrentStaff.Id)
            {
                throw new Exception("当前员工为主员工, 不可删除");
            }

            SimpleContentDialogCreateOptions dialog = new()
            {
                Title = "是否删除员工!",
                Content = $"员工名称: {SelectedStaff.Name}",
                PrimaryButtonText = "确认",
                CloseButtonText = "取消"
            };

            //弹窗
            var dialogResult = await contentDialogService.ShowSimpleDialogAsync(dialog);

            //等待确认
            if (dialogResult != ContentDialogResult.Primary) return;

            //更新数据库
            //await _staffService.RemoveAsync(order);

            //删除
            var index = StaffCollection.RemoveAtMatchedIndex(x => x.Id == userStaff.Id);
            SelectedStaff = StaffCollection.TryGetElementWithOffset(index, -1);

            snackbarService.ShowSuccess($"已删除员工: {userStaff.Name}");
        }
        catch (Exception ex)
        {
            snackbarService.ShowError(ex);
            logger.LogError(ex, "删除员工时发生错误");
        }
    }

    /// <summary>
    ///     添加
    /// </summary>
    /// <returns></returns>
    [RelayCommand]
    private async Task AddStaffAsync()
    {
        try
        {
            var creatorVm = new StaffCreatorObservableViewModel();

            var content = new SimpleContentDialogCreateOptions
            {
                Title = "新增员工",
                Content = new StaffCreatorView { DataContext = creatorVm },
                PrimaryButtonText = "创建",
                CloseButtonText = "取消"
            };

            //弹窗
            var dialogResult = await contentDialogService.ShowSimpleDialogAsync(content);
            if (dialogResult != ContentDialogResult.Primary) return;


            //更新数据库
            await staffService.AddAsync(creatorVm.Staff, CurrentUser);
            
            StaffCollection.Add(creatorVm.Staff, x => x.Id != creatorVm.Staff.Id);
            dbService.AutoUpdate(creatorVm.Staff);
            SelectedStaff = creatorVm.Staff;
        }
        catch (Exception ex)
        {
            snackbarService.ShowError(ex);
            logger.LogError(ex, "添加员工时发生错误");
        }
    }

    #endregion

    #region --职位--

    /// <summary>
    ///     加载已绑定的职位
    /// </summary>
    /// <returns></returns>
    [RelayCommand]
    private async Task ReloadPositionAsync()
    {
        try
        {
            ArgumentNullException.ThrowIfNull(SelectedStaff);

            var positions = await staffPositionService.GetAllByStaffAsync(SelectedStaff);

            PositionCollection = positions.ToObservableCollection();
            PositionCollection.ForEach(x=>dbService.AutoUpdate(x));
            SelectedPosition = PositionCollection.FirstOrDefault();
        }
        catch (Exception ex)
        {
            snackbarService.ShowError(ex);
            logger.LogError(ex, "加载职位时发生错误");
        }
    }


    /// <summary>
    ///     取消绑定职位
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    [RelayCommand]
    private async Task UnbindingPositionAsync(StaffPositionViewModel position)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(SelectedStaff);

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
            await staffPositionService.RemoveStaffAsync(position.Type, SelectedStaff);

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
    ///     绑定职位
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [RelayCommand]
    private async Task BindingPositionAsync(StaffPositionType type)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(SelectedStaff);

            //更新数据库
            var position = await staffPositionService.SetStaffAsync(type, SelectedStaff);

            PositionCollection.Add(position, x => x.Type != position.Type);
            dbService.AutoUpdate(position);
            SelectedPosition = position;
        }
        catch (Exception ex)
        {
            snackbarService.ShowError(ex);
            logger.LogError(ex, "职位添加失败, 类型:{type}", type);
        }
    }

    #endregion
}