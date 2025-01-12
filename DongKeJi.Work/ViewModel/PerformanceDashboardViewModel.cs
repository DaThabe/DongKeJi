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
using DongKeJi.Work.UI.View.Customer;
using DongKeJi.Work.UI.View.Order;
using DongKeJi.Work.ViewModel.Consume;
using DongKeJi.Work.ViewModel.Customer;
using DongKeJi.Work.ViewModel.Order;
using DongKeJi.Work.ViewModel.Staff;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Wpf.Ui;
using Wpf.Ui.Controls;
using Wpf.Ui.Extensions;

namespace DongKeJi.Work.ViewModel;

/// <summary>
///     明细管理
/// </summary>
[Inject(ServiceLifetime.Transient)]
public partial class PerformanceDashboardViewModel(
    ILogger<PerformanceDashboardViewModel> logger,
    IContentDialogService contentDialogService,
    ISnackbarService snackbarService,
    IWorkDbService dbService,
    ICoreContext coreContext,
    IWorkContext workContext,
    IStaffService staffService,
    ICustomerService customerService,
    IOrderService orderService,
    IConsumeService consumeService
) : LazyInitializeViewModel
{
    #region --上下文属性--

    /// <summary>
    ///     当前用户
    /// </summary>
    public UserViewModel CurrentUser =>
        coreContext.CurrentUser ?? throw new ArgumentNullException(nameof(CurrentUser), "缺少必要参数: 当前用户实例为空");

    public StaffViewModel CurrentStaff =>
        workContext.CurrentStaff ?? throw new ArgumentNullException(nameof(CurrentUser), "缺少必要参数: 当前员工实例为空");


    /// <summary>
    ///     当前选中销售
    /// </summary>
    [ObservableProperty] private StaffViewModel? _selectedSalesperson;

    /// <summary>
    ///     当前选中机构
    /// </summary>
    [ObservableProperty] private CustomerViewModel? _selectedCustomer;

    /// <summary>
    ///     当前选中订单
    /// </summary>
    [ObservableProperty] private OrderViewModel? _selectedOrder;

    /// <summary>
    ///     当前选中划扣
    /// </summary>
    [ObservableProperty] private ConsumeViewModel? _selectedConsume;


    /// <summary>
    /// 销售列表
    /// </summary>
    [ObservableProperty] private ObservableCollection<StaffViewModel> _salespersonCollection = [];

    /// <summary>
    ///     机构列表
    /// </summary>
    [ObservableProperty] private ObservableCollection<CustomerViewModel> _customerCollection = [];

    /// <summary>
    ///     订单列表
    /// </summary>
    [ObservableProperty] private ObservableCollection<OrderViewModel> _orderCollection = [];

    /// <summary>
    ///     划扣列表
    /// </summary>
    [ObservableProperty] private ObservableCollection<ConsumeViewModel> _consumeCollection = [];

    #endregion

    #region --初始化&默认行为--

    protected override async Task OnInitializationAsync(CancellationToken cancellation = default)
    {
        await ReloadCustomerCommand.ExecuteAsync(null);
        await ReloadSalespersonCommand.ExecuteAsync(null);
    }

    partial void OnSelectedCustomerChanged(CustomerViewModel? value)
    {
        SelectedOrder = null;
        OrderCollection.Clear();

        if (value is null) return;

        _ = ReloadOrderCommand.ExecuteAsync(null);
    }

    async partial void OnSelectedOrderChanged(OrderViewModel? value)
    {
        SelectedConsume = null;
        ConsumeCollection.Clear();

        if (value is null) return;

        if (SelectedOrder is not null)
        {
            _selectedSalesperson = await orderService.FindSalespersonAsync(SelectedOrder);
        }

        _ = ReloadConsumeCommand.ExecuteAsync(null);
    }

    partial void OnSelectedSalespersonChanged(StaffViewModel? value)
    {
        if (value is null || SelectedOrder is null) return;
        orderService.ChangeSalespersonAsync(SelectedOrder, value);
    }

    #endregion

    #region --销售--

    /// <summary>
    ///     刷新销售
    /// </summary>
    /// <returns></returns>
    [RelayCommand]
    private async Task ReloadSalespersonAsync()
    {
        try
        {
            var salespersonList = await staffService.FindAllByPositionTypeAsync(StaffPositionType.Salesperson);

            SalespersonCollection = salespersonList.ToObservableCollection();
            SelectedSalesperson = SalespersonCollection.FirstOrDefault();
        }
        catch (Exception ex)
        {
            snackbarService.ShowError(ex);
            logger.LogError(ex, "加载机构时发生错误");
        }
    }


    #endregion

    #region --机构--

    /// <summary>
    ///     刷新
    /// </summary>
    /// <returns></returns>
    [RelayCommand]
    private async Task ReloadCustomerAsync()
    {
        try
        {
            var customers = await customerService.GetAllByStaffIdAsync(CurrentStaff);

            CustomerCollection = customers.ToObservableCollection();
            SelectedCustomer = CustomerCollection.FirstOrDefault();
        }
        catch (Exception ex)
        {
            snackbarService.ShowError(ex);
            logger.LogError(ex, "加载机构时发生错误");
        }
    }

    /// <summary>
    ///     新增
    /// </summary>
    /// <returns></returns>
    [RelayCommand]
    private async Task AddCustomerAsync()
    {
        try
        {
            var customerCreatorVm = new CustomerCreatorObservableViewModel();
            var dialogContent = new SimpleContentDialogCreateOptions
            {
                Title = "新增机构",
                Content = new CustomerCreatorView { DataContext = customerCreatorVm },
                PrimaryButtonText = "创建",
                CloseButtonText = "取消"
            };

            //弹窗
            var dialogResult = await contentDialogService.ShowSimpleDialogAsync(dialogContent);

            //等待确认
            if (dialogResult != ContentDialogResult.Primary) return;

            //更新数据库
            await customerService.AddAsync(customerCreatorVm.Customer, CurrentStaff);

            //更新界面
            CustomerCollection.Add(customerCreatorVm.Customer);
            SelectedCustomer = customerCreatorVm.Customer;

            snackbarService.ShowSuccess($"已创建机构: {customerCreatorVm.Customer.Name} - {customerCreatorVm.Customer.Id}");
        }
        catch (Exception ex)
        {
            snackbarService.ShowError(ex);
            logger.LogError(ex, "新增机构时发生错误");
        }
    }

    /// <summary>
    ///     删除
    /// </summary>
    /// <param name="customer"></param>
    /// <returns></returns>
    [RelayCommand]
    private async Task RemoveCustomerAsync(CustomerViewModel customer)
    {
        try
        {
            SimpleContentDialogCreateOptions dialog = new()
            {
                Title = "是否删除机构",
                Content = $"机构名称: {customer.Name}",
                PrimaryButtonText = "确认",
                CloseButtonText = "取消"
            };

            //弹窗
            var dialogResult = await contentDialogService.ShowSimpleDialogAsync(dialog);

            //等待确认
            if (dialogResult != ContentDialogResult.Primary) return;

            //更新数据库
            await customerService.RemoveAsync(customer);

            //删除
            var index = CustomerCollection.RemoveAtMatchedIndex(x => x.Id == customer.Id);
            SelectedCustomer = CustomerCollection.TryGetElementWithOffset(index, -1);

            snackbarService.ShowSuccess($"已删除机构: {customer.Name} - {customer.Id}");
        }
        catch (Exception ex)
        {
            snackbarService.ShowError(ex);
            logger.LogError(ex, "删除机构时发生错误\n机构信息: {customer}", customer);
        }
    }

    #endregion

    #region --订单--

    /// <summary>
    ///     刷新数据
    /// </summary>
    /// <returns></returns>
    [RelayCommand]
    private async Task ReloadOrderAsync()
    {
        try
        {
            ArgumentNullException.ThrowIfNull(SelectedCustomer);

            var orderVMs = await orderService.GetAllByCustomerIdAsync(SelectedCustomer);
            
            OrderCollection = orderVMs.ToObservableCollection();
            OrderCollection.ForEach(x => dbService.AutoUpdate(x));
            SelectedOrder = OrderCollection.FirstOrDefault();
        }
        catch (Exception ex)
        {
            snackbarService.ShowError(ex);
            logger.LogError(ex, "刷新订单时发生错误");
        }
    }

    /// <summary>
    ///     新增
    /// </summary>
    /// <returns></returns>
    [RelayCommand]
    private async Task AddOrderAsync()
    {
        try
        {
            ArgumentNullException.ThrowIfNull(SelectedCustomer);

            var salespersonList = await staffService
                .FindAllByUserAndPositionTypeAsync(CurrentUser, StaffPositionType.Salesperson);

            var creatorVm = new OrderCreatorObservableViewModel(salespersonList);

            var content = new SimpleContentDialogCreateOptions
            {
                Title = "新增订单",
                Content = new OrderCreatorView { DataContext = creatorVm },
                PrimaryButtonText = "创建",
                CloseButtonText = "取消"
            };

            //弹窗
            var dialogResult = await contentDialogService.ShowSimpleDialogAsync(content);

            //等待确认
            if (dialogResult != ContentDialogResult.Primary) return;

            if (creatorVm.SelectedSalesperson is null)
            {
                throw new Exception("订单新增失败, 未设置订单关联销售");
            }

            //更新数据库
            await orderService.AddAsync(creatorVm.Order, creatorVm.SelectedSalesperson, SelectedCustomer);

            //更新界面
            OrderCollection.Add(creatorVm.Order);
            dbService.AutoUpdate(creatorVm.Order);
            SelectedOrder = creatorVm.Order;

            snackbarService.ShowSuccess($"已添加订单: {creatorVm.Order.Name} - {creatorVm.Order.Id}");
        }
        catch (Exception ex)
        {
            snackbarService.ShowError(ex);
            logger.LogError(ex, "新增订单时发生错误");
        }
    }

    /// <summary>
    ///     删除
    /// </summary>
    /// <param name="order"></param>
    /// <returns></returns>
    [RelayCommand]
    private async Task RemoveOrderAsync(OrderViewModel? order)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(order);


            SimpleContentDialogCreateOptions dialog = new()
            {
                Title = "是否删除订单",
                Content = $"订单名称: {order.Name}",
                PrimaryButtonText = "确认",
                CloseButtonText = "取消"
            };

            //弹窗
            var dialogResult = await contentDialogService.ShowSimpleDialogAsync(dialog);

            //等待确认
            if (dialogResult != ContentDialogResult.Primary) return;

            //更新数据库
            await orderService.RemoveAsync(order);

            //删除
            var index = OrderCollection.RemoveAtMatchedIndex(x => x.Id == order.Id);
            SelectedOrder = OrderCollection.TryGetElementWithOffset(index, -1);

            snackbarService.ShowSuccess($"已删除订单: {order.Name} - {order.Id}");
        }
        catch (Exception ex)
        {
            snackbarService.ShowError(ex);
            logger.LogError(ex, "删除订单时发生错误\n订单信息: {order}", order);
        }
    }

    #endregion

    #region --划扣--

    /// <summary>
    ///     刷新
    /// </summary>
    /// <returns></returns>
    [RelayCommand]
    private async Task ReloadConsumeAsync()
    {
        try
        {
            List<ConsumeViewModel> consumes = [];

            switch (SelectedOrder)
            {
                case OrderTimingViewModel:
                    consumes.AddRange(await consumeService.GetAllByTimingOrderAsync(SelectedOrder)); break;

                case OrderCountingViewModel:
                    consumes.AddRange(await consumeService.GetAllByCountingOrderAsync(SelectedOrder)); break;

                case OrderMixingViewModel:
                    consumes.AddRange(await consumeService.GetAllByMixingOrderAsync(SelectedOrder)); break;
            }

            ConsumeCollection = consumes.ToObservableCollection();
            SelectedConsume = consumes.FirstOrDefault();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "刷新划扣时发生错误");
        }
    }

    /// <summary>
    ///     新增
    /// </summary>
    /// <returns></returns>
    [RelayCommand]
    private async Task AddConsumeAsync()
    {
        try
        {
            ConsumeViewModel consumeVm = SelectedOrder switch
            {
                OrderTimingViewModel => new ConsumeTimingViewModel { ConsumeDays = 1 },
                OrderCountingViewModel => new ConsumeCountingViewModel { ConsumeCounts = 1 },
                OrderMixingViewModel => new ConsumeMixingViewModel { ConsumeDays = 1, ConsumeCounts = 0 },
                _ => throw new Exception($"未知订单类型, 订单信息: {SelectedOrder}")
            };

            if (consumeVm is null) throw new Exception($"划扣创建失败, 订单类型不明确, 订单类型: {SelectedOrder.GetType()}");

            await consumeService.AddAsync(consumeVm, SelectedOrder, CurrentStaff);

            //更新界面
            ConsumeCollection.Add(consumeVm);
            dbService.AutoUpdate(consumeVm);
            SelectedConsume = consumeVm;

            snackbarService.ShowSuccess($"已创建划扣: {consumeVm.Title} - {consumeVm.Id}");
        }
        catch (Exception ex)
        {
            snackbarService.ShowError(ex);
            logger.LogError(ex, "新增划扣时发生错误");
        }
    }

    /// <summary>
    ///     删除
    /// </summary>
    /// <param name="consume"></param>
    /// <returns></returns>
    [RelayCommand]
    private async Task RemoveConsumeAsync(ConsumeViewModel consume)
    {
        try
        {
            //更新数据库
            await consumeService.RemoveAsync(consume);

            //删除
            var index = ConsumeCollection.RemoveAtMatchedIndex(x => x.Id == consume.Id);
            SelectedConsume = ConsumeCollection.TryGetElementWithOffset(index, -1);

            snackbarService.ShowSuccess($"已删除划扣: {consume.Title} - {consume.Id}");
        }
        catch (Exception ex)
        {
            snackbarService.ShowError(ex);
            logger.LogError(ex, "删除划扣时发生错误");
        }
    }

    #endregion
}