using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DongKeJi.Common.Inject;
using DongKeJi.Extensions;
using DongKeJi.UI;
using DongKeJi.ViewModel;
using DongKeJi.ViewModel.User;
using DongKeJi.Work.Model.Entity.Staff;
using DongKeJi.Work.Service;
using DongKeJi.Work.UI.View.Staff;
using DongKeJi.Work.ViewModel.Common.Staff;
using DongKeJi.Work.ViewModel.Staff;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Wpf.Ui;
using Wpf.Ui.Controls;
using Wpf.Ui.Extensions;
using StaffPositionViewModel = DongKeJi.Work.ViewModel.Staff.StaffPositionViewModel;
using StaffViewModel = DongKeJi.Work.ViewModel.Staff.StaffViewModel;
using UserViewModel = DongKeJi.Core.ViewModel.User.UserViewModel;

namespace DongKeJi.Work.ViewModel;

/// <summary>
///     员工管理
/// </summary>
[Inject(ServiceLifetime.Transient)]
public partial class StaffDashboardObservableViewModel(
    ILogger<StaffDashboardObservableViewModel> logger,
    ISnackbarService snackbarService,
    IContentDialogService contentDialogService,
    IWorkContext workContext,
    IStaffService staffService,
    IStaffPositionService staffPositionService
) : LazyInitializeObservableViewModel, IStaffDashboardContext
{
    #region --上下文属性--

    /// <summary>
    ///     用户
    /// </summary>
    public UserViewModel LoginUser => workContext.LoginUser;

    /// <summary>
    ///     员工
    /// </summary>
    [ObservableProperty] private StaffViewModel _staff = workContext.PrimaryStaff;

    /// <summary>
    ///     员工列表
    /// </summary>
    [ObservableProperty] private ObservableCollection<StaffViewModel> _staffList = [];


    /// <summary>
    ///     职位
    /// </summary>
    [ObservableProperty] private StaffPositionViewModel _position = StaffPositionViewModel.Default;

    /// <summary>
    ///     职位列表
    /// </summary>
    [ObservableProperty] private ObservableCollection<StaffPositionViewModel> _positionList = [];

    #endregion

    #region --默认行为&初始化--

    protected override async Task OnInitializationAsync(CancellationToken cancellation = default)
    {
        await ReloadStaffCommand.ExecuteAsync(null);
    }

    partial void OnStaffChanged(StaffViewModel? value)
    {
        Staff = value ?? StaffViewModel.Default;

        Position = StaffPositionViewModel.Default;
        PositionList.Clear();


        if (Staff != StaffViewModel.Default)
        {
            ReloadPositionCommand.ExecuteAsync(null);
        }
    }

    #endregion

    #region --员工--

    /// <summary>
    ///     创建员工
    /// </summary>
    /// <returns></returns>
    private async Task<StaffViewModel?> CreateStaffAsync()
    {
        var staffCreatorViewModel = new StaffCreatorObservableViewModel();

        var content = new SimpleContentDialogCreateOptions
        {
            Title = "新增员工",
            Content = new StaffCreatorView { DataContext = staffCreatorViewModel },
            PrimaryButtonText = "创建",
            CloseButtonText = "取消"
        };

        //弹窗

        while (true)
        {
            var dialogResult = await contentDialogService.ShowSimpleDialogAsync(content);

            if (dialogResult != ContentDialogResult.Primary) return null;

            if (string.IsNullOrWhiteSpace(staffCreatorViewModel.Staff.Name))
            {
                snackbarService.ShowWarning("名称不可为空");
                continue;
            }

            break;
        }

        //等待确认
        return staffCreatorViewModel.Staff;
    }

    /// <summary>
    ///     刷新员工
    /// </summary>
    /// <returns></returns>
    [RelayCommand]
    private async Task ReloadStaffAsync()
    {
        try
        {
            var result = await staffService.FindAllByUserAsync(LoginUser);

            StaffList = result.ToObservableCollection();
            Staff = StaffList.FirstOrDefault() ?? StaffViewModel.Default;
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
            SimpleContentDialogCreateOptions dialog = new()
            {
                Title = "是否删除员工!",
                Content = $"员工名称: {Staff.Name}",
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
            var index = StaffList.RemoveAtMatchedIndex(x => x.Id == userStaff.Id);
            Staff = StaffList.TryGetElementWithOffset(index, -1) ?? StaffViewModel.Default;

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
        var staff = await CreateStaffAsync();
        if (staff is null) return;

        try
        {
            //更新数据库
            await staffService.AddAsync(staff, LoginUser);
            StaffList.Add(staff, x => x.Id != staff.Id);
            Staff = staff;
        }
        catch (Exception ex)
        {
            snackbarService.ShowError(ex);
            logger.LogError(ex, "员工添加失败, 名称:{type}, Id:{id}", staff.Name, staff.Id);
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
            var positions = await staffPositionService.FindAllByStaffAsync(Staff);

            PositionList = positions.ToObservableCollection();
            Position = PositionList.FirstOrDefault() ?? StaffPositionViewModel.Default;
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
            await staffPositionService.UnbindingAsync(position.Type, Staff);

            //删除
            var index = PositionList.RemoveAtMatchedIndex(x => x.Type == position.Type);
            Position = PositionList.TryGetElementWithOffset(index, -1) ?? StaffPositionViewModel.Default;

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
            //更新数据库
            var position = await staffPositionService.BindingAsync(type, Staff);

            if (position is not null)
            {
                PositionList.Add(position, x => x.Type != position.Type);
                Position = position;
            }
        }
        catch (Exception ex)
        {
            snackbarService.ShowError(ex);
            logger.LogError(ex, "职位添加失败, 类型:{type}", type);
        }
    }

    #endregion
}