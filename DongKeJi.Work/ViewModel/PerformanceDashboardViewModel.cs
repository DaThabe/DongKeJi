using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DongKeJi.Common.Extensions;
using DongKeJi.Common.Inject;
using DongKeJi.Common.UI;
using DongKeJi.Common.ViewModel;
using DongKeJi.ViewModel;
using DongKeJi.ViewModel.User;
using DongKeJi.Work.Model.Entity.Staff;
using DongKeJi.Work.Service;
using DongKeJi.Work.UI.View.Common.Customer;
using DongKeJi.Work.UI.View.Common.Order;
using DongKeJi.Work.ViewModel.Common.Consume;
using DongKeJi.Work.ViewModel.Common.Customer;
using DongKeJi.Work.ViewModel.Common.Order;
using DongKeJi.Work.ViewModel.Common.Staff;
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

    IWorkContext workContext,

    IStaffService staffService,
    ICustomerService customerService,
    IOrderService orderService,
    IConsumeService consumeService

    ) : LazyInitializeViewModel, IPerformanceDashboardContext
{
    #region --上下文属性--

    /// <summary>
    /// 用户
    /// </summary>
    [ObservableProperty] private UserViewModel _user = workContext.User;

    /// <summary>
    /// 员工
    /// </summary>
    [ObservableProperty] private StaffViewModel _staff = StaffViewModel.Empty;

    /// <summary>
    /// 机构
    /// </summary>
    [ObservableProperty] private CustomerViewModel _customer = CustomerViewModel.Empty;

    /// <summary>
    /// 订单
    /// </summary>
    [ObservableProperty] private OrderViewModel _order = OrderViewModel.Empty;

    /// <summary>
    /// 划扣
    /// </summary>
    [ObservableProperty] private ConsumeViewModel _consume = ConsumeViewModel.Empty;


    /// <summary>
    ///     机构列表
    /// </summary>
    [ObservableProperty] private ObservableCollection<CustomerViewModel> _customerList = [];

    /// <summary>
    ///     订单列表
    /// </summary>
    [ObservableProperty] private ObservableCollection<OrderViewModel> _orderList = [];

    /// <summary>
    ///     划扣列表
    /// </summary>
    [ObservableProperty] private ObservableCollection<ConsumeViewModel> _consumeList = [];

    #endregion

    #region --初始化&默认行为--

    protected override async Task OnInitializationAsync(CancellationToken cancellation = default)
    {
        if (User == UserViewModel.Empty) throw new Exception("未登录用户, 无法初始化明细页面");

        var result = await staffService.FindAllByUserAsync(User, cancellation);
        var staffs = result.ToList();

        if (staffs.Count <= 0 || !staffs.Any(x => x.IsPrimaryAccount))
        {
            var staff = new StaffViewModel { Name = User.Name, IsPrimaryAccount = true };

            if (await staffService.AddAsync(staff, User, cancellation))
            {
                staffs.Add(staff);
            }
        }

        Staff = staffs.Find(x => x.IsPrimaryAccount) ?? StaffViewModel.Empty;
        if (Staff == StaffViewModel.Empty)
        {
            throw new Exception("未能创建或加载用户");
        }

        await ReloadCustomerCommand.ExecuteAsync(null);
    }

    partial void OnStaffChanged(StaffViewModel? value)
    {
        _customer = CustomerViewModel.Empty;
        _order = OrderViewModel.Empty;
        _consume = ConsumeViewModel.Empty;

        CustomerList.Clear();
        OrderList.Clear();
        ConsumeList.Clear();


        _staff = value ?? StaffViewModel.Empty;
        _ = ReloadOrderCommand.ExecuteAsync(null);
    }

    partial void OnCustomerChanged(CustomerViewModel? value)
    {
        _order = OrderViewModel.Empty;
        _consume = ConsumeViewModel.Empty;

        OrderList.Clear();
        ConsumeList.Clear();


        _customer = value ?? CustomerViewModel.Empty;
        _ = ReloadOrderCommand.ExecuteAsync(null);
    }

    partial void OnOrderChanged(OrderViewModel? value)
    {
        _consume = ConsumeViewModel.Empty;
        ConsumeList.Clear();

        _order = value ?? OrderViewModel.Empty;
        _ = ReloadConsumeCommand.ExecuteAsync(null);
    }

    #endregion


    #region --机构--

    /// <summary>
    ///     创建机构
    /// </summary>
    /// <returns></returns>
    public async Task<CustomerViewModel?> CreateCustomerAsync()
    {
        var customerCreatorVm = new CustomerCreatorViewModel();

        var content = new SimpleContentDialogCreateOptions
        {
            Title = "新增机构",
            Content = new CustomerCreatorView { DataContext = customerCreatorVm },
            PrimaryButtonText = "创建",
            CloseButtonText = "取消"
        };

        //弹窗

        while (true)
        {
            var dialogResult = await contentDialogService.ShowSimpleDialogAsync(content);

            if (dialogResult != ContentDialogResult.Primary) return null;

            if (string.IsNullOrWhiteSpace(customerCreatorVm.Customer.Name))
            {
                snackbarService.ShowWarning("名称不可为空");
                continue;
            }

            break;
        }

        //等待确认
        return customerCreatorVm.Customer;
    }


    /// <summary>
    ///     刷新
    /// </summary>
    /// <returns></returns>
    [RelayCommand]
    private async Task ReloadCustomerAsync()
    {
        try
        {
            var customers = await customerService.GetAllByStaffIdAsync(Staff);

            CustomerList = customers.ToObservableCollection();
            Customer = CustomerList.FirstOrDefault() ?? CustomerViewModel.Empty;
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
            var customerVm = await CreateCustomerAsync();
            if (customerVm is null) return;

            //更新数据库
            await customerService.AddAsync(customerVm, Staff);

            //更新界面
            CustomerList.Add(customerVm);
            Customer = customerVm;

            snackbarService.ShowSuccess($"已创建机构: {customerVm.Name} - {customerVm.Id}");
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
            var index = CustomerList.RemoveAtMatchedIndex(x => x.Id == customer.Id);
            Customer = CustomerList.TryGetElementWithOffset(index, -1) ?? CustomerViewModel.Empty;

            snackbarService.ShowSuccess($"已删除机构: {customer.Name} - {customer.Id}");
        }
        catch (Exception ex)
        {
            snackbarService.ShowError(ex);
            logger.LogError(ex, "删除机构时发生错误");
        }
    }

    #endregion

    #region --订单--

    /// <summary>
    ///     创建订单
    /// </summary>
    /// <returns></returns>
    public async ValueTask<OrderViewModel?> CreateOrderAsync()
    {
        var salespersonList = await staffService.FindAllByUserAndPositionTypeAsync(User, StaffPositionType.Salesperson);
        var orderCreatorVm = new OrderCreatorViewModel(salespersonList);

        var content = new SimpleContentDialogCreateOptions
        {
            Title = "新增订单",
            Content = new OrderCreatorView { DataContext = orderCreatorVm },
            PrimaryButtonText = "创建",
            CloseButtonText = "取消"
        };

        //弹窗
        var dialogResult = await contentDialogService.ShowSimpleDialogAsync(content);

        //等待确认
        if (dialogResult != ContentDialogResult.Primary) return null;

        return orderCreatorVm.Order;
    }


    /// <summary>
    ///     刷新数据
    /// </summary>
    /// <returns></returns>
    [RelayCommand]
    private async Task ReloadOrderAsync()
    {
        try
        {
            var orderVMs = await orderService.GetAllByCustomerIdAsync(Customer);

            OrderList = orderVMs.ToObservableCollection();
            Order = OrderList.FirstOrDefault() ?? OrderViewModel.Empty;
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
            var orderVm = await CreateOrderAsync();
            if (orderVm is null) return;

            //更新数据库
            await orderService.AddAsync(orderVm, Staff, Customer);

            //更新界面
            OrderList.Add(orderVm);
            Order = orderVm;

            snackbarService.ShowSuccess($"已添加订单: {orderVm.Name} - {orderVm.Id}");
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
    private async Task RemoveOrderAsync(OrderViewModel order)
    {
        try
        {
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
            var index = OrderList.RemoveAtMatchedIndex(x => x.Id == order.Id);
            Order = OrderList.TryGetElementWithOffset(index, -1) ?? OrderViewModel.Empty;

            snackbarService.ShowSuccess($"已删除订单: {order.Name} - {order.Id}");
        }
        catch (Exception ex)
        {
            snackbarService.ShowError(ex);
            logger.LogError(ex, "删除订单时发生错误");
        }
    }

    #endregion

    #region --划扣--

    /// <summary>
    ///     创建划扣
    /// </summary>
    /// <returns></returns>
    private ConsumeViewModel? CreateConsume()
    {
        return Order switch
        {
            TimingOrderViewModel => new TimingConsumeViewModel { ConsumeDays = 1 },
            CountingOrderViewModel => new CountingConsumeViewModel { ConsumeCounts = 1 },
            MixingOrderViewModel => new MixingConsumeViewModel { ConsumeDays = 1, ConsumeCounts = 0 },
            _ => null
        };
    }


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

            switch (Order)
            {
                case TimingOrderViewModel:
                    consumes.AddRange(await consumeService.GetAllByTimingOrderAsync(Order)); break;

                case CountingOrderViewModel:
                    consumes.AddRange(await consumeService.GetAllByCountingOrderAsync(Order)); break;

                case MixingOrderViewModel:
                    consumes.AddRange(await consumeService.GetAllByMixingOrderAsync(Order)); break;
            }

            ConsumeList = consumes.ToObservableCollection();
            Consume = consumes.FirstOrDefault() ?? ConsumeViewModel.Empty;
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
            var consumeVm = CreateConsume();
            if (consumeVm is null) throw new Exception($"划扣创建失败, 订单类型不明确, 订单类型: {Order.GetType()}");

            await consumeService.AddAsync(consumeVm, Order, Staff);

            //更新界面
            ConsumeList.Add(consumeVm);
            Consume = consumeVm;

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
            var index = ConsumeList.RemoveAtMatchedIndex(x => x.Id == consume.Id);
            Consume = ConsumeList.TryGetElementWithOffset(index, -1) ?? ConsumeViewModel.Empty;

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