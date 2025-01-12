using AutoMapper;
using DongKeJi.Database;
using DongKeJi.Entity;
using DongKeJi.Inject;
using DongKeJi.ViewModel;
using DongKeJi.Work.Model;
using DongKeJi.Work.Model.Entity.Consume;
using DongKeJi.Work.Model.Entity.Customer;
using DongKeJi.Work.Model.Entity.Order;
using DongKeJi.Work.Model.Entity.Staff;
using DongKeJi.Work.ViewModel.Consume;
using DongKeJi.Work.ViewModel.Customer;
using DongKeJi.Work.ViewModel.Order;
using DongKeJi.Work.ViewModel.Staff;
using Microsoft.Extensions.DependencyInjection;

namespace DongKeJi.Work.Service;

public interface IWorkDbService
{
    internal IServiceProvider ServiceProvider { get; }
}

[Inject(ServiceLifetime.Singleton, typeof(IWorkDbService))]
internal class WorkDbService(IServiceProvider services) : IWorkDbService
{
    public IServiceProvider ServiceProvider { get; } = services;
}


public static class CoreDatabaseExtensions
{
    private static IAutoUpdateBuilder<TViewModel> AutoUpdate<TEntity, TViewModel>(this IWorkDbService dbService, TViewModel viewModel)
        where TEntity : EntityBase
        where TViewModel : IEntityViewModel
    {
        var mapper = dbService.ServiceProvider.GetRequiredService<IMapper>();
        var dbContext = dbService.ServiceProvider.GetRequiredService<WorkDbContext>();

        return dbContext.AutoUpdate<TEntity, TViewModel>(viewModel, mapper);
    }


    public static IAutoUpdateBuilder<StaffViewModel> AutoUpdate(this IWorkDbService dbService, StaffViewModel vm)
        => dbService.AutoUpdate<StaffEntity, StaffViewModel>(vm);

    public static IAutoUpdateBuilder<StaffPositionViewModel> AutoUpdate(this IWorkDbService dbService, StaffPositionViewModel vm)
        => dbService.AutoUpdate<StaffPositionEntity, StaffPositionViewModel>(vm);

    public static IAutoUpdateBuilder<CustomerViewModel> AutoUpdate(this IWorkDbService dbService, CustomerViewModel vm)
        => dbService.AutoUpdate<CustomerEntity, CustomerViewModel>(vm);

    public static IAutoUpdateBuilder<OrderViewModel> AutoUpdate(this IWorkDbService dbService, OrderViewModel vm)
        => dbService.AutoUpdate<OrderEntity, OrderViewModel>(vm);

    public static IAutoUpdateBuilder<ConsumeViewModel> AutoUpdate(this IWorkDbService dbService, ConsumeViewModel vm)
        => dbService.AutoUpdate<ConsumeEntity, ConsumeViewModel>(vm);
}