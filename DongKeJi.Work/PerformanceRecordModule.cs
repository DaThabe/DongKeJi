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
    public string Title => "������ϸ��¼";
    public string Describe => "�ṩ����������ϸ��¼����";

    public void Configure(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddAutoInject<PerformanceRecordModule>();
            services.AddHostedService<HostedService>();

            //���ݿ�
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
            logger.LogError(e, "��ʼ���칫ģ��ʱ��������");
        }
    }


    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }


    /// <summary>
    /// ��ʼ���˵�
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private void InitMenu()
    {
        var menu = mainFrameService.AddMenu<PerformanceDashboardView>(SymbolRegular.Note24, "������ϸ");
        menu.AddChildMenu<StaffDashboardView>(SymbolRegular.People24, "Ա������");
    }

    /// <summary>
    /// ��ʼ��Ա���˻�
    /// </summary>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private async Task InitStaffAccount(CancellationToken cancellation = default)
    {
        if (workContext.User == UserViewModel.Empty) throw new Exception("δ��¼�û�, �޷���ʼ����ϸҳ��");

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
            throw new Exception("δ�ܴ���������û�");
        }
    }

    /// <summary>
    /// ��ʼ��Ա��ְλ
    /// </summary>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private async Task InitStaffPosition(CancellationToken cancellation = default)
    {
        //��������ְλ (���û��
        var salesperson = await staffPositionService.FindByTypeAsync(StaffPositionType.Salesperson, cancellation);
        if (salesperson is null)
        {
            salesperson = new StaffPositionViewModel
            {
                Title = "����",
                Type = StaffPositionType.Salesperson
            };

            await staffPositionService.SetAsync(salesperson, cancellation);
        }

        //�������ְλ (���û��
        var designer = await staffPositionService.FindByTypeAsync(StaffPositionType.Designer, cancellation);
        if (designer is null)
        {
            salesperson = new StaffPositionViewModel
            {
                Title = "���",
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