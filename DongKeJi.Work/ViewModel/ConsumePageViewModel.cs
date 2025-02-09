﻿using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DongKeJi.Core;
using DongKeJi.Core.ViewModel.User;
using DongKeJi.Database;
using DongKeJi.Extensions;
using DongKeJi.Inject;
using DongKeJi.UI;
using DongKeJi.ViewModel;
using DongKeJi.Work.Model.Entity.Order;
using DongKeJi.Work.Model.Entity.Staff;
using DongKeJi.Work.Service;
using DongKeJi.Work.ViewModel.Compose;
using DongKeJi.Work.ViewModel.Staff;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Wpf.Ui;

namespace DongKeJi.Work.ViewModel;


/// <summary>
///     明细管理
/// </summary>
[Inject(ServiceLifetime.Transient)]
public partial class ConsumePageViewModel(
    ILogger<CustomerPageViewModel> logger,
    ISnackbarService snackbarService,
    IWorkDatabase dbService,
    IStaffService staffService,
    ICoreModule coreModule,
    IWorkModule workModule,
    IOrderService orderService,
    IConsumeService consumeService
) : LazyInitializeViewModel
{
    #region --上下文属性--

    /// <summary>
    ///     当前用户
    /// </summary>
    public UserViewModel CurrentUser =>
        coreModule.CurrentUser ?? throw new ArgumentNullException(nameof(CurrentUser), "缺少必要参数: 当前用户实例为空");

        /// <summary>
        /// 当前员工
        /// </summary>
    public StaffViewModel CurrentStaff =>
        workModule.CurrentStaff ?? throw new ArgumentNullException(nameof(CurrentUser), "缺少必要参数: 当前员工实例为空");


    /// <summary>
    /// 所有订单
    /// </summary>
    [ObservableProperty] private StaffViewModel? _selectedDesigner;

    /// <summary>
    /// 所有划扣
    /// </summary>
    [ObservableProperty] private ObservableCollection<StaffViewModel> _designerCollection = [];


    /// <summary>
    /// 显示时间
    /// </summary>
    [ObservableProperty] private DateTime _displayDate;

    /// <summary>
    /// 当前时间
    /// </summary>
    [ObservableProperty] private DateTime _selectedDate;


    /// <summary>
    /// 所有划扣
    /// </summary>
    [ObservableProperty] private DesignerCustomerOrderConsumeViewModel? _selectedConsume;

    /// <summary>
    /// 所有待办划扣
    /// </summary>
    [ObservableProperty] private ObservableCollection<DesignerCustomerOrderConsumeViewModel> _todoConsumeCollection = [];

    /// <summary>
    /// 所有划扣
    /// </summary>
    [ObservableProperty] private ObservableCollection<DesignerCustomerOrderConsumeViewModel> _consumeCollection = [];

    #endregion

    #region --初始化&默认行为--

    protected override async ValueTask OnInitializationAsync(CancellationToken cancellation = default)
    {
        try
        {
            //加载所有设计师
            var designer = await staffService.GetAllByUserAndPositionTypeAsync(CurrentUser, StaffPositionType.Designer, cancellation: cancellation);
            DesignerCollection = designer.ToObservableCollection();
            DesignerCollection.ForEach(x => dbService.AutoUpdate(x));
        }
        catch (Exception ex)
        {
            snackbarService.ShowError(ex);
            logger.LogError(ex, "加载设计师时发生错误");
        }

        SelectedDate = DateTime.Now;
        DisplayDate = SelectedDate;

    }

    async partial void OnSelectedDateChanged(DateTime value)
    {
        await ReloadConsumeCommand.ExecuteAsync(null);
    }

    #endregion

    #region --划扣--

    /// <summary>
    /// 刷新
    /// </summary>
    /// <returns></returns>
    [RelayCommand]
    private async Task ReloadConsumeAsync()
    {
        try
        {
            ArgumentNullException.ThrowIfNull(CurrentStaff);

            List<DesignerCustomerOrderConsumeViewModel> consumeCollection = [];
            List<DesignerCustomerOrderConsumeViewModel> todoConsumeCollection = [];

            //活跃订单
            var orders = (await orderService.GetAllAsync())
                .Where(x => x.State == OrderState.Active);

            //遍历订单
            foreach (var order in orders)
            {
                //查询订单机构
                var customer = await orderService.GetCustomerAsync(order);

                //订单划扣
                var consumes = (await consumeService.GetAllByOrderAsync(order))
                    .Where(x => x.CreateTime.Date == SelectedDate.Date)
                    .ToList();

                //没有划扣
                if (consumes.Count <= 0)
                {
                    var todoConsume = order.Type.CreateConsume(SelectedDate);
                    if (todoConsume is null) continue;

                    //设计师不存在
                    var existsDesigner = DesignerCollection.FirstOrDefault(x => x.Id == CurrentStaff.Id);
                    if (existsDesigner is null) continue;

                    var consumeDesignerCustomerOrder = new DesignerCustomerOrderConsumeViewModel(
                        existsDesigner, customer, order, todoConsume);

                    //添加待办划扣
                    todoConsumeCollection.Add(consumeDesignerCustomerOrder);
                }

                //遍历划扣
                foreach (var consume in consumes)
                {
                    //查询设计师
                    var designer = await consumeService.GetDesignerAsync(consume);
                    
                    //设计师不存在
                    var existsDesigner = DesignerCollection.FirstOrDefault(x => x.Id == designer.Id);
                    if (existsDesigner is null) continue;

                    //添加划扣元素
                    consumeCollection.Add(new DesignerCustomerOrderConsumeViewModel(
                        existsDesigner, customer, order, consume));

                    //注册划扣自动保存
                    dbService.AutoUpdate(consume);
                }

                //注册机构子自动保存
                dbService.AutoUpdate(customer);
                //注册订单自动保存
                dbService.AutoUpdate(order);
            }

            TodoConsumeCollection = todoConsumeCollection.ToObservableCollection();
            ConsumeCollection = consumeCollection.ToObservableCollection();
        }
        catch (Exception ex)
        {
            snackbarService.ShowError(ex);
            logger.LogError(ex, "加载划扣时发生错误");
        }
    }

    /// <summary>
    /// 完成待办
    /// </summary>
    /// <returns></returns>
    [RelayCommand]
    private async Task FinishTodoConsumeAsync(DesignerCustomerOrderConsumeViewModel viewModel)
    {
        try
        {
            await consumeService.AddAsync(viewModel.Consume, viewModel.Order, viewModel.Designer);
            
            TodoConsumeCollection.Remove(viewModel);
            dbService.AutoUpdate(viewModel.Consume);
            ConsumeCollection.Add(viewModel);
        }
        catch (Exception ex)
        {
            snackbarService.ShowError(ex);
            logger.LogError(ex, "完成待办时发生错误");
        }
    }

    /// <summary>
    /// 删除划扣
    /// </summary>
    /// <param name="designerCustomerOrderConsume"></param>
    /// <returns></returns>
    [RelayCommand]
    private async Task RemoveConsumeAsync(DesignerCustomerOrderConsumeViewModel designerCustomerOrderConsume)
    {
        try
        {
            await consumeService.RemoveAsync(designerCustomerOrderConsume.Consume);
            designerCustomerOrderConsume.Consume.ReleaseAutoUpdate();

            ConsumeCollection.Remove(designerCustomerOrderConsume);
            TodoConsumeCollection.Add(designerCustomerOrderConsume);
        }
        catch (Exception ex)
        {
            snackbarService.ShowError(ex);
            logger.LogError(ex, "删除划扣时发生错误");
        }
    }

    /// <summary>
    /// 点击划扣
    /// </summary>
    /// <param name="designerCustomerOrderConsume"></param>
    /// <returns></returns>
    [RelayCommand]
    private void ClickConsume(DesignerCustomerOrderConsumeViewModel designerCustomerOrderConsume)
    {
        SelectedConsume = designerCustomerOrderConsume;
    }


    #endregion

    #region --时间--

    /// <summary>
    /// 前一天
    /// </summary>
    /// <returns></returns>
    [RelayCommand]
    private void PrevDate()
    {
        SelectedDate -= TimeSpan.FromDays(1);
        DisplayDate = SelectedDate;
    }

    /// <summary>
    /// 前一天
    /// </summary>
    /// <returns></returns>
    [RelayCommand]
    private void ToDayDate()
    {
        DisplayDate = DateTime.Now;
        SelectedDate  = DateTime.Now;
    }

    /// <summary>
    /// 下一天
    /// </summary>
    /// <returns></returns>
    [RelayCommand]
    private void NextDate( )
    {
        SelectedDate += TimeSpan.FromDays(1);
        DisplayDate = SelectedDate;
    }


    #endregion"
}