using DongKeJi.Core;
using DongKeJi.Core.Service;
using DongKeJi.Inject;
using DongKeJi.Module;
using DongKeJi.Work.Model;
using DongKeJi.Work.Model.Entity.Staff;
using DongKeJi.Work.Service;
using DongKeJi.Work.UI.View;
using DongKeJi.Work.ViewModel.Staff;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Wpf.Ui.Controls;

namespace DongKeJi.Work;


/// <summary>
/// 办公模块
/// </summary>
public class WorkModule : IModule
{
    private static readonly ModuleMetaInfo ModuleMetaInfo = new()
    {
        Id = Guid.NewGuid(),
        Name = "DongKeJi.Work",
        Version = new Version(0, 0, 1),
        Title = "办公",
        Developers = ["DaThabe"],
        Describe = """
                   ### 办公模块
                   - 机构明细记录
                   """,
        CreatedAt = new DateTime(2024, 9, 17),
        ReleaseDate = new DateTime(2025, 1, 12),
        Dependencies =
        [
            typeof(CoreModule).Assembly.GetName(),
        ]
    };


    public IModuleMetaInfo MetaInfo => ModuleMetaInfo;

    public void Configure(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddAutoInject<WorkModule>();
            services.AddHostedService<HostedService>();

            //数据库
            services.AddDbContext<WorkDbContext>();
            //AutoMapper
            services.AddAutoMapper(typeof(WorkMapperProfile));
        });
    }
}

file class HostedService(
    ILogger<HostedService> logger,
    IMainFrameService mainFrameService,
    ICoreContext coreContext,
    IWorkDbService workDbService,
    IWorkContext workContext,
    IStaffService staffService,
    IStaffPositionService staffPositionService
) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            var menu = mainFrameService.AddMenu<PerformanceDashboardView>(SymbolRegular.Note24, "机构明细");
            menu.AddChildMenu<StaffDashboardView>(SymbolRegular.People24, "员工管理");
            menu.AddChildMenu<StaffPositionDashboardView>(SymbolRegular.VideoPeople32, "职位管理");

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
    ///     初始化员工账户
    /// </summary>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private async Task InitStaffAccount(CancellationToken cancellation = default)
    {
        if (coreContext.CurrentUser is null)
        {
            throw new Exception("未登录用户, 无法初始化明细页面");
        }

        try
        {
            var primaryStaff = await staffService.GetBindingPrimaryIdAsync(coreContext.CurrentUser, cancellation);
            await staffService.SetCurrentAsync(primaryStaff, cancellation);

            var staffVm =  await staffService.FindByIdAsync(primaryStaff, cancellation);
            await staffService.SetCurrentAsync(staffVm, cancellation);
            workDbService.AutoUpdate(staffVm);
        }
        catch
        {
            try
            {
                var result = await staffService.FindAllByUserAsync(coreContext.CurrentUser, cancellation);
                var staffVm = result.FirstOrDefault();

                ArgumentNullException.ThrowIfNull(staffVm);

                await staffService.SetCurrentAsync(staffVm, cancellation);
                await staffService.BindingPrimaryAsync(coreContext.CurrentUser, staffVm, cancellation);
                workDbService.AutoUpdate(staffVm);
            }
            catch
            {
                var staffVm = new StaffViewModel { Name = coreContext.CurrentUser.Name };
                await staffService.AddAsync(staffVm, coreContext.CurrentUser, cancellation);

                await staffService.SetCurrentAsync(staffVm, cancellation);
                await staffService.BindingPrimaryAsync(coreContext.CurrentUser, staffVm, cancellation);
                workDbService.AutoUpdate(staffVm);
            }
        }
    }

    /// <summary>
    ///     初始化员工职位
    /// </summary>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private async Task InitStaffPosition(CancellationToken cancellation = default)
    {
        //创建销售职位 (如果没有
        try
        {
            await staffPositionService.FindByTypeAsync(StaffPositionType.Salesperson, cancellation);
        }
        catch
        {
            await staffPositionService.SetAsync(new StaffPositionViewModel
            {
                Title = "销售",
                Type = StaffPositionType.Salesperson

            }, cancellation);
        }

        try
        {
            await staffPositionService.FindByTypeAsync(StaffPositionType.Designer, cancellation);
        }
        catch 
        {
            await staffPositionService.SetAsync(new StaffPositionViewModel
            {
                Title = "设计",
                Type = StaffPositionType.Designer

            }, cancellation);
        }
    }


    //private async Task DebugAsync(CancellationToken cancellationToken)
    //{
    //    var user = applicationContext.User;
    //    var staff = await staffRepository.FindAll(user, cancellationToken);

    //    if (staff is null)
    //    {
    //        staff = new StaffViewModel();
    //        await staffRepository.AddAsync(staff, user, cancellationToken);
    //    }

    //    var customer = new CustomerViewModel();
    //    await customerRepository.AddAsync(customer, staff, cancellationToken);

    //    var order = new CountingOrderViewModel();
    //    await orderRepository.AddAsync(order, staff, customer, cancellationToken);

    //    var consume = new CountingConsumeViewModel();
    //    await consumeRepository.AddAsync(consume, order, staff, cancellationToken);
    //}
}