using CommunityToolkit.Mvvm.ComponentModel;
using DongKeJi.ViewModel;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using DongKeJi.Extensions;
using DongKeJi.Inject;
using DongKeJi.Work.Service;
using DongKeJi.Work.ViewModel.Compose;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Wpf.Ui;
using DongKeJi.UI;
using DongKeJi.Work.ViewModel.Staff;
using System.Windows.Controls;
using DongKeJi.Work.UI.View.Consume;
using DongKeJi.Work.UI.View.Staff;
using DongKeJi.Work.ViewModel.Consume;
using Wpf.Ui.Controls;
using Wpf.Ui.Extensions;

namespace DongKeJi.Work.ViewModel;


[Inject(ServiceLifetime.Transient)]
internal partial class WagesDashboardViewModel(
    IServiceProvider services,
    ILogger<WagesDashboardViewModel> logger,
    ISnackbarService snackbarService,
    IContentDialogService contentDialogService,
    IWorkModule workModule,
    IWorkConfig workConfig,
    IWagesService wagesService,
    IOrderService orderService,
    IConsumeService consumeService
    ) : LazyInitializeViewModel
{
    #region --上下文属性--

    /// <summary>
    /// 当前员工
    /// </summary>
    public StaffViewModel CurrentStaff =>
        workModule.CurrentStaff ?? throw new ArgumentNullException(nameof(CurrentStaff), "缺少必要参数: 当前员工实例为空");


    [ObservableProperty] private double _底薪;
    [ObservableProperty] private double _提成百分比;
    [ObservableProperty] private double _总工资;


    /// <summary>
    /// 显示的日期
    /// </summary>
    [ObservableProperty] private DateTime _displayDate;

    /// <summary>
    /// 选中的日期
    /// </summary>
    [ObservableProperty] private DateTime _selectedDate;

    [ObservableProperty] private MonthlyCustomerConsumePrice _monthlyConsume = null!;

    /// <summary>
    /// 划扣明细集合
    /// </summary>
    [ObservableProperty] private ObservableCollection<CustomerOrderConsumePriceViewModel> _collection = [];


    #endregion

    #region --默认行为&初始化--

    protected override async ValueTask OnInitializationAsync(CancellationToken cancellation = default)
    {
        SelectedDate = DateTime.Now;
        DisplayDate = SelectedDate;
        MonthlyConsume = new MonthlyCustomerConsumePrice(Collection);

        try
        {
            _底薪 = await workConfig.BasicSalary.GetAsync(cancellation: cancellation);
        }
        catch
        {
            _底薪 = 3000;
        }

        try
        {
            _提成百分比 = await workConfig.CommissionPercentage.GetAsync(cancellation: cancellation);
        }
        catch
        {
            _提成百分比 = 10;
        }

        await ReloadCommand.ExecuteAsync(null);
    }

    async partial void On提成百分比Changed(double value)
    {
        if (!IsInitialized) return;

        CalcPrice();
        await workConfig.CommissionPercentage.SetAsync(value);
    }

    async partial void On底薪Changed(double value)
    {
        if (!IsInitialized) return;

        CalcPrice();
        await workConfig.BasicSalary.SetAsync(value);
    }

    async partial void OnSelectedDateChanged(DateTime value)
    {
        if (_lastDateTime.EqualsYearMonth(value))
        {
            return;
        }
        _lastDateTime = value;
        await ReloadCommand.ExecuteAsync(null);
    }

    #endregion

    #region --提成--

    [RelayCommand]
    public void CalcPrice()
    {
        var 系数 = 提成百分比 / 100;
        Collection.ForEach(x => x.UpdatePrice(系数));
        MonthlyConsume.UpdatePrice(系数);

        总工资 = 底薪 + MonthlyConsume.Price;
    }

    /// <summary>
    /// 重载
    /// </summary>
    /// <returns></returns>
    [RelayCommand]
    private async Task ReloadAsync()
    {
        try
        {
            List<CustomerOrderConsumePriceViewModel> items = [];

            var orders = await orderService.GetAllByStaffAsync(CurrentStaff);
            foreach (var order in orders)
            {
                var customer = await orderService.GetCustomerAsync(order);
                var consumes = (await consumeService.GetAllByOrderAsync(order))
                    .Where(x => x.CreateTime.EqualsYearMonth(SelectedDate))
                    .ToArray();

                if (consumes.Length <= 0) continue;
                items.Add(new CustomerOrderConsumePriceViewModel(SelectedDate, customer, order, consumes));
            }

            Collection = items.ToObservableCollection();
            MonthlyConsume = new MonthlyCustomerConsumePrice(Collection);
            
            CalcPrice();
        }
        catch (Exception ex)
        {
            snackbarService.ShowError(ex);
            logger.LogError(ex, "加载明细时发生错误");
        }
    }

    /// <summary>
    /// 导出明细
    /// </summary>
    [RelayCommand]
    private async Task ExportAsync()
    {
        try
        {
            var vm = services.GetRequiredService<ConsumeExporterViewModel>();
            vm.Date = SelectedDate;
            vm.ExportContent = await wagesService.ExportMonthlyConsumeAsync(CurrentStaff, SelectedDate);

            var content = new SimpleContentDialogCreateOptions
            {
                Title = "导出明细",
                Content = new ConsumeExporterView { DataContext = vm },
                CloseButtonText = "关闭"
            };

            //弹窗
            await contentDialogService.ShowSimpleDialogAsync(content);
        }
        catch (Exception ex)
        {
            snackbarService.ShowError(ex);
            logger.LogError(ex, "导出明细时发生错误");
        }
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
        SelectedDate = SelectedDate.AddMonths(-1);
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
        SelectedDate = DateTime.Now;
    }

    /// <summary>
    /// 下一天
    /// </summary>
    /// <returns></returns>
    [RelayCommand]
    private void NextDate()
    {
        SelectedDate = SelectedDate.AddMonths(1);
        DisplayDate = SelectedDate;
    }


    #endregion"



    private DateTime _lastDateTime = DateTime.MinValue;
}