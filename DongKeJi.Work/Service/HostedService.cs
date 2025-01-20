using DongKeJi.Core;
using DongKeJi.Core.Service;
using DongKeJi.Database;
using DongKeJi.Work.Model;
using DongKeJi.Work.Model.Entity.Staff;
using DongKeJi.Work.UI.View;
using DongKeJi.Work.ViewModel.Staff;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Wpf.Ui.Controls;

namespace DongKeJi.Work.Service;

internal class HostedService(
    ILogger<HostedService> logger,
    IMainFrameService mainFrameService,
    ICoreModule coreModule,
    IWorkDatabase workDbService,
    IStaffService staffService,
    WorkDbContext dbContext,
    IStaffPositionService staffPositionService
) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            await dbContext.UnitOfWorkAsync(async t =>
            {
                await dbContext.Database.MigrateAsync(cancellationToken: cancellationToken);

            }, cancellationToken);

        }
        catch (Exception e)
        {
            logger.LogError(e, "办公数据库迁移失败, 已回滚");
        }

        try
        {
            await InitStaffAccount(cancellationToken);
            await InitStaffPosition(cancellationToken);


            mainFrameService.AddMenu<WorkPage>(SymbolRegular.Briefcase20, "办公", builder =>
            {
                builder.AddChild<CustomerPage>(SymbolRegular.BuildingPeople24, "机构");
                builder.AddChild<ConsumePage>(SymbolRegular.NotepadEdit16, "划扣");
                builder.AddChild<WagesPage>(SymbolRegular.ArrowTrendingLines24, "提成");
                builder.AddChild<StaffPage>(SymbolRegular.People24, "员工");
                builder.AddChild<StaffPositionPage>(SymbolRegular.VideoPerson16, "职位");
            });
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
        if (coreModule.CurrentUser is null)
        {
            throw new Exception("未登录用户, 无法初始化明细页面");
        }

        try
        {
            var primaryStaff = await staffService.GetPrimaryIdAsync(coreModule.CurrentUser, cancellation);
            await staffService.SetPrimaryAsync(primaryStaff, cancellation);

            var staffVm = await staffService.GetByIdAsync(primaryStaff, cancellation);
            await staffService.SetPrimaryAsync(staffVm, cancellation);
            workDbService.AutoUpdate(staffVm);
        }
        catch
        {
            try
            {
                var result = await staffService.GetAllByUserAsync(coreModule.CurrentUser, cancellation);
                var staffVm = result.FirstOrDefault();

                ArgumentNullException.ThrowIfNull(staffVm);

                await staffService.SetPrimaryAsync(staffVm, cancellation);
                await staffService.SetPrimaryToUserAsync(staffVm, coreModule.CurrentUser, cancellation);
                workDbService.AutoUpdate(staffVm);
            }
            catch
            {
                var staffVm = new StaffViewModel { Name = coreModule.CurrentUser.Name };
                await staffService.AddAsync(staffVm, coreModule.CurrentUser, cancellation);

                await staffService.SetPrimaryAsync(staffVm, cancellation);
                await staffService.SetPrimaryToUserAsync(staffVm, coreModule.CurrentUser, cancellation);
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
            await staffPositionService.GetByTypeAsync(StaffPositionType.Salesperson, cancellation);
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
            await staffPositionService.GetByTypeAsync(StaffPositionType.Designer, cancellation);
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