using DongKeJi.Common.Module;
using DongKeJi.Service;
using DongKeJi.ViewModel;
using DongKeJi.ViewModel.User;
using DongKeJi.Work.Model;
using DongKeJi.Work.Model.Entity.Staff;
using DongKeJi.Work.Service;
using DongKeJi.Work.UI.View;
using DongKeJi.Work.ViewModel;
using DongKeJi.Work.ViewModel.Common.Staff;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading;
using DongKeJi.Common.Inject;
using Microsoft.Extensions.Logging;
using Wpf.Ui.Controls;
using StaffPositionViewModel = DongKeJi.Work.ViewModel.Common.Staff.StaffPositionViewModel;

namespace DongKeJi.Work;

public class PerformanceRecordModule : IModule
{
    public string Title => "服务明细记录";
    public string Describe => "提供机构服务明细记录功能";

    public void Configure(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddAutoInject<PerformanceRecordModule>();
            services.AddHostedService<HostedService>();

            //数据库
            services.AddDbContext<PerformanceRecordDbContext>();
            //AutoMapper
            services.AddAutoMapper(typeof(PerformanceRecordMapperProfile));
        });
    }
}

file class HostedService(
    ILogger<HostedService> logger,
    IApplicationContext applicationContext,
    IMainFrameService mainFrameService,
    IWorkContext workContext,
    IStaffService staffService,
    IStaffPositionService staffPositionService
) : IHostedService
{

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            InitMenu();
            await InitStaffAccount(cancellationToken);
            await InitStaffPosition(cancellationToken);
        }
        catch (Exception e)
        {
            logger.LogError(e, "初始化办公模块时发生错误");
        }
    }


    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }


    /// <summary>
    /// 初始化菜单
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private void InitMenu()
    {
        var menu = mainFrameService.AddMenu<PerformanceDashboardView>(SymbolRegular.Note24, "机构明细");
        menu.AddChildMenu<StaffDashboardView>(SymbolRegular.People24, "员工管理");
    }

    /// <summary>
    /// 初始化员工账户
    /// </summary>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private async Task InitStaffAccount(CancellationToken cancellation = default)
    {
        if (workContext.User == UserViewModel.Empty) throw new Exception("未登录用户, 无法初始化明细页面");

        var result = await staffService.FindAllByUserAsync(workContext.User, cancellation);
        var staffs = result.ToList();

        if (staffs.Count <= 0 || !staffs.Any(x => x.IsPrimaryAccount))
        {
            var staff = new StaffViewModel { Name = workContext.User.Name, IsPrimaryAccount = true };

            if (await staffService.AddAsync(staff, workContext.User, cancellation))
            {
                staffs.Add(staff);
            }
        }

        workContext.Staff = staffs.Find(x => x.IsPrimaryAccount) ?? StaffViewModel.Empty;

        if (workContext.Staff == StaffViewModel.Empty)
        {
            throw new Exception("未能创建或加载用户");
        }
    }

    /// <summary>
    /// 初始化员工职位
    /// </summary>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private async Task InitStaffPosition(CancellationToken cancellation = default)
    {
        //创建销售职位 (如果没有
        var salesperson = await staffPositionService.FindByTypeAsync(StaffPositionType.Salesperson, cancellation);
        if (salesperson is null)
        {
            salesperson = new StaffPositionViewModel
            {
                Title = "销售",
                Type = StaffPositionType.Salesperson
            };

            await staffPositionService.SetAsync(salesperson, cancellation);
        }

        //创建设计职位 (如果没有
        var designer = await staffPositionService.FindByTypeAsync(StaffPositionType.Designer, cancellation);
        if (designer is null)
        {
            salesperson = new StaffPositionViewModel
            {
                Title = "设计",
                Type = StaffPositionType.Designer
            };

            await staffPositionService.SetAsync(salesperson, cancellation);
        }
    }


    //private async Task DebugAsync(CancellationToken cancellationToken)
    //{
    //    var user = applicationContext.User;
    //    var staff = await staffService.FindAll(user, cancellationToken);

    //    if (staff is null)
    //    {
    //        staff = new StaffViewModel();
    //        await staffService.AddAsync(staff, user, cancellationToken);
    //    }

    //    var customer = new CustomerViewModel();
    //    await customerService.AddAsync(customer, staff, cancellationToken);

    //    var order = new CountingOrderViewModel();
    //    await orderService.AddAsync(order, staff, customer, cancellationToken);

    //    var consume = new CountingConsumeViewModel();
    //    await consumeService.AddAsync(consume, order, staff, cancellationToken);
    //}
}