using DongKeJi.Common.Inject;
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
using Microsoft.Extensions.Logging;
using Wpf.Ui.Controls;

namespace DongKeJi.Work;


public class WorkModule : IModule
{
    public string Title => "������ϸ��¼";
    public string Describe => "�ṩ����������ϸ��¼����";

    public void Configure(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddAutoInject<WorkModule>();
            services.AddHostedService<HostedService>();

            //���ݿ�
            services.AddDbContext<WorkDbContext>();
            //AutoMapper
            services.AddAutoMapper(typeof(WorkMapperProfile));
        });
    }
}

file class HostedService(
    ILogger<HostedService> logger,
    IApplicationContext applicationContext,
    IMainFrameService mainFrameService,
    IWorkContext workContext,
    IStaffRepository staffRepository,
    IStaffPositionService staffPositionService
) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            var menu = mainFrameService.AddMenu<PerformanceDashboardView>(SymbolRegular.Note24, "������ϸ");
            menu.AddChildMenu<StaffDashboardView>(SymbolRegular.People24, "Ա������");
            menu.AddChildMenu<StaffPositionDashboardView>(SymbolRegular.VideoPeople32, "ְλ����");

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
    ///     ��ʼ��Ա���˻�
    /// </summary>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private async Task InitStaffAccount(CancellationToken cancellation = default)
    {
        if (workContext.LoginUser == UserViewModel.Empty) throw new Exception("δ��¼�û�, �޷���ʼ����ϸҳ��");

        var result = await staffRepository.FindAllByUserAsync(workContext.LoginUser, cancellation);
        var staffs = result.ToList();

        if (staffs.Count <= 0 || !staffs.Any(x => x.IsPrimaryAccount))
        {
            var staff = new StaffViewModel { Name = workContext.LoginUser.Name, IsPrimaryAccount = true };
            await staffRepository.AddAsync(staff, workContext.LoginUser, cancellation);
            staffs.Add(staff);
        }

        workContext.PrimaryStaff = staffs.Find(x => x.IsPrimaryAccount) ?? StaffViewModel.Empty;

        if (workContext.PrimaryStaff == StaffViewModel.Empty) throw new Exception("δ�ܴ���������û�");
    }

    /// <summary>
    ///     ��ʼ��Ա��ְλ
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