using System.Collections.ObjectModel;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DongKeJi.Core;
using DongKeJi.Core.ViewModel.User;
using DongKeJi.Extensions;
using DongKeJi.Inject;
using DongKeJi.UI;
using DongKeJi.ViewModel;
using DongKeJi.Work.Model.Entity.Order;
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
///     机构管理
/// </summary>
[Inject(ServiceLifetime.Transient)]
public partial class CustomerDashboardViewModel(
    ILogger<CustomerDashboardViewModel> logger,
    IContentDialogService contentDialogService,
    ISnackbarService snackbarService,
    IWorkDatabase dbService,
    ICoreModule coreModule,
    IWorkModule workModule,
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
        coreModule.CurrentUser ?? throw new ArgumentNullException(nameof(CurrentUser), "缺少必要参数: 当前用户实例为空");

    public StaffViewModel CurrentStaff =>
        workModule.CurrentStaff ?? throw new ArgumentNullException(nameof(CurrentUser), "缺少必要参数: 当前员工实例为空");


    /// <summary>
    ///     当前选中机构
    /// </summary>
    [ObservableProperty] private CustomerViewModel? _selectedCustomer;

    /// <summary>
    ///     当前选中订单
    /// </summary>
    [ObservableProperty] private OrderSalespersonViewModel? _selectedOrder;

    /// <summary>
    ///     当前选中划扣
    /// </summary>
    [ObservableProperty] private ConsumeDesignerViewModel? _selectedConsume;


    /// <summary>
    /// 销售集合
    /// </summary>
    [ObservableProperty] private ObservableCollection<StaffViewModel> _salespersonCollection = [];

    /// <summary>
    /// 设计集合
    /// </summary>
    [ObservableProperty] private ObservableCollection<StaffViewModel> _designerCollection = [];

    /// <summary>
    ///     订单状态集合
    /// </summary>
    [ObservableProperty] private ObservableCollection<OrderState> _orderStateCollection =
    [
        OrderState.Ready, OrderState.Active, OrderState.Paused, OrderState.Expired, OrderState.Cancel
    ];

    /// <summary>
    ///     机构集合
    /// </summary>
    [ObservableProperty] private ObservableCollection<CustomerViewModel> _customerCollection = [];

    /// <summary>
    ///     订单集合
    /// </summary>
    [ObservableProperty] private ObservableCollection<OrderSalespersonViewModel> _orderCollection = [];

    /// <summary>
    ///     划扣集合
    /// </summary>
    [ObservableProperty] private ObservableCollection<ConsumeDesignerViewModel> _consumeCollection = [];

    #endregion

    #region --初始化&默认行为--

    protected override async ValueTask OnInitializationAsync(CancellationToken cancellation = default)
    {
        try
        {
            var salespersonList = await staffService.GetAllByPositionTypeAsync(StaffPositionType.Salesperson, cancellation: cancellation);

            SalespersonCollection = salespersonList.ToObservableCollection();
            SalespersonCollection.ForEach(x => dbService.AutoUpdate(x));
        }
        catch (Exception ex)
        {
            snackbarService.ShowError(ex);
            logger.LogError(ex, "加载销售时发生错误");
            return;
        }

        try
        {
            var salespersonList = await staffService.GetAllByPositionTypeAsync(StaffPositionType.Designer, cancellation: cancellation);

            DesignerCollection = salespersonList.ToObservableCollection();
            DesignerCollection.ForEach(x => dbService.AutoUpdate(x));
        }
        catch (Exception ex)
        {
            snackbarService.ShowError(ex);
            logger.LogError(ex, "加载设计时发生错误");
            return;
        }

        await ReloadCustomerCommand.ExecuteAsync(null);
    }

    partial void OnSelectedCustomerChanged(CustomerViewModel? value)
    {
        if (value is null) return;
        _ = ReloadOrderCommand.ExecuteAsync(null);
    }

    partial void OnSelectedOrderChanged(OrderSalespersonViewModel? value)
    {
        if (value is null) return;
        _ = ReloadConsumeCommand.ExecuteAsync(null);
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
            CustomerCollection.ForEach(x => dbService.AutoUpdate(x));
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
            var customerCreatorVm = new CustomerCreatorViewModel();
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
            dbService.AutoUpdate(customerCreatorVm.Customer);

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

            List<OrderSalespersonViewModel> orderSalespersons = [];

            var orderVMs = await orderService.GetAllByCustomerAsync(SelectedCustomer);
            foreach (var order in orderVMs)
            {
                var salesperson = await orderService.GetSalespersonAsync(order);

                var existsSalesperson = SalespersonCollection.FirstOrDefault(x => x.Id == salesperson.Id);
                if (existsSalesperson is null) continue;

                var orderSalesperson = new OrderSalespersonViewModel(order, existsSalesperson);
                orderSalesperson.SalespersonChanged += async e =>
                {
                    await orderService.SetSalespersonAsync(order, e);
                };

                orderSalespersons.Add(orderSalesperson);

                dbService.AutoUpdate(salesperson);
                dbService.AutoUpdate(order);
            }

            OrderCollection = orderSalespersons.ToObservableCollection();
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
                .GetAllByUserAndPositionTypeAsync(CurrentUser, StaffPositionType.Salesperson);

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
            var orderSalesperson = new OrderSalespersonViewModel(creatorVm.Order, creatorVm.SelectedSalesperson);
            dbService.AutoUpdate(creatorVm.Order);

            //更新界面
            OrderCollection.Add(orderSalesperson);
            SelectedOrder = orderSalesperson;

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
    /// <param name="orderSalesperson"></param>
    /// <returns></returns>
    [RelayCommand]
    private async Task RemoveOrderAsync(OrderSalespersonViewModel? orderSalesperson)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(orderSalesperson);


            SimpleContentDialogCreateOptions dialog = new()
            {
                Title = "是否删除订单",
                Content = $"订单名称: {orderSalesperson.Order.Name}",
                PrimaryButtonText = "确认",
                CloseButtonText = "取消"
            };

            //弹窗
            var dialogResult = await contentDialogService.ShowSimpleDialogAsync(dialog);

            //等待确认
            if (dialogResult != ContentDialogResult.Primary) return;

            //更新数据库
            await orderService.RemoveAsync(orderSalesperson.Order);

            //删除
            var index = OrderCollection.RemoveAtMatchedIndex(x => x.Order.Id == orderSalesperson.Order.Id);
            SelectedOrder = OrderCollection.TryGetElementWithOffset(index, -1);

            snackbarService.ShowSuccess($"已删除订单: {orderSalesperson.Order.Name} - {orderSalesperson.Order.Id}");
        }
        catch (Exception ex)
        {
            snackbarService.ShowError(ex);
            logger.LogError(ex, "删除订单时发生错误\n订单信息: {order}", orderSalesperson);
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
            ArgumentNullException.ThrowIfNull(SelectedOrder);

            List<ConsumeDesignerViewModel> consumeDesigners = [];
            var consumes = await consumeService.FindAllConsumeAsync(SelectedOrder.Order);

            foreach (var consume in consumes)
            {
                var designer = await consumeService.GetDesignerAsync(consume);
                var existsDesigner = DesignerCollection.FirstOrDefault(x => x.Id == designer.Id);
                if (existsDesigner == null) continue;

                var consumeDesigner = new ConsumeDesignerViewModel(consume, existsDesigner);
                consumeDesigner.DesignerChanged += e =>
                {
                    consumeService.SetDesignerAsync(consume, e);
                };

                consumeDesigners.Add(consumeDesigner);
                dbService.AutoUpdate(designer);
            }

            ConsumeCollection = consumeDesigners.ToObservableCollection();
            SelectedConsume = ConsumeCollection.FirstOrDefault();
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
            ArgumentNullException.ThrowIfNull(SelectedOrder);

            var consume = SelectedOrder.Order.Type.CreateConsume(DateTime.Now);
            if (consume is null) throw new Exception($"划扣创建失败, 订单类型不明确, 订单类型: {SelectedOrder.GetType()}");

            //更新数据库
            await consumeService.AddAsync(consume, SelectedOrder.Order, CurrentStaff);
            var consumeDesigner = new ConsumeDesignerViewModel(consume, CurrentStaff);
            dbService.AutoUpdate(consume);

            //更新界面
            ConsumeCollection.Add(consumeDesigner);
            SelectedConsume = consumeDesigner;

            snackbarService.ShowSuccess($"已创建划扣: {consume.Title} - {consume.Id}");
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
    /// <param name="consumeDesigner"></param>
    /// <returns></returns>
    [RelayCommand]
    private async Task RemoveConsumeAsync(ConsumeDesignerViewModel? consumeDesigner)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(consumeDesigner);

            //更新数据库
            await consumeService.RemoveAsync(consumeDesigner.Consume);

            //删除
            var index = ConsumeCollection.RemoveAtMatchedIndex(x => x.Consume.Id == consumeDesigner.Consume.Id);
            SelectedConsume = ConsumeCollection.TryGetElementWithOffset(index, -1);

            snackbarService.ShowSuccess($"已删除划扣: {consumeDesigner.Consume.Title} - {consumeDesigner.Consume.Id}");
        }
        catch (Exception ex)
        {
            snackbarService.ShowError(ex);
            logger.LogError(ex, "删除划扣时发生错误");
        }
    }

    #endregion
}