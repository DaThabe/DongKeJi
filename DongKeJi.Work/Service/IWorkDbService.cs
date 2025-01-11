using AutoMapper;
using DongKeJi.Core.Service;
using DongKeJi.Database;
using DongKeJi.Entity;
using DongKeJi.Inject;
using DongKeJi.Service;
using DongKeJi.ViewModel;
using DongKeJi.Work.Model;
using DongKeJi.Work.Model.Entity.Consume;
using DongKeJi.Work.Model.Entity.Customer;
using DongKeJi.Work.Model.Entity.Order;
using DongKeJi.Work.Model.Entity.Staff;
using DongKeJi.Work.ViewModel;
using DongKeJi.Work.ViewModel.Consume;
using DongKeJi.Work.ViewModel.Customer;
using DongKeJi.Work.ViewModel.Order;
using DongKeJi.Work.ViewModel.Staff;
using Microsoft.Extensions.DependencyInjection;

namespace DongKeJi.Work.Service;

public interface IWorkDbService : IDbService
{
    internal IServiceProvider ServiceProvider { get; }
}

[Inject(ServiceLifetime.Singleton, typeof(ICoreDbService))]
internal class WorkDbService(IServiceProvider services) : IWorkDbService
{
    public IServiceProvider ServiceProvider { get; } = services;
}


public static class CoreDatabaseExtensions
{
    private static async ValueTask UpdateAsync<TEntity, TViewModel>(
        this IWorkDbService dbService,
        TViewModel viewModel, 
        CancellationToken cancellation = default)
        where TEntity : EntityBase
        where TViewModel : IWorkEntityViewModel
    {
        var dbContext = dbService.ServiceProvider.GetRequiredService<WorkDbContext>();
        var mapper = dbService.ServiceProvider.GetRequiredService<IMapper>();

        await dbContext.UpdateAsync<TEntity, TViewModel>(viewModel, mapper, cancellation);
    }

    public static void RegisterAutoUpdate<TEntity, TViewModel>(this IWorkDbService dbService, TViewModel viewModel)
        where TViewModel : IWorkEntityViewModel
    {
        viewModel.PropertyChanged += async (_, _) =>
        {
            await dbService.UpdateAsync<TEntity, TViewModel>(viewModel);
        };
    }


    /// <summary>
    /// 更新数据
    /// </summary>
    /// <param name="dbService"></param>
    /// <param name="staff"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    public static ValueTask UpdateAsync(this IWorkDbService dbService, StaffViewModel staff, CancellationToken cancellation = default)
    {
        return dbService.UpdateAsync<StaffEntity, StaffViewModel>(staff, cancellation);
    }

    public static ValueTask UpdateAsync(this IWorkDbService dbService, StaffPositionViewModel position, CancellationToken cancellation = default)
    {
        return dbService.UpdateAsync<StaffPositionEntity, StaffPositionViewModel>(position, cancellation);
    }

    public static ValueTask UpdateAsync(this IWorkDbService dbService, CustomerViewModel customer, CancellationToken cancellation = default)
    {
        return dbService.UpdateAsync<CustomerEntity, CustomerViewModel>(customer, cancellation);
    }

    public static ValueTask UpdateAsync(this IWorkDbService dbService, OrderViewModel order, CancellationToken cancellation = default)
    {
        return dbService.UpdateAsync<OrderEntity, OrderViewModel>(order, cancellation);
    }

    public static ValueTask UpdateAsync(this IWorkDbService dbService, ConsumeViewModel consume, CancellationToken cancellation = default)
    {
        return dbService.UpdateAsync<ConsumeEntity, ConsumeViewModel>(consume, cancellation);
    }

    public static ValueTask RegisterAutoUpdate(this IWorkDbService dbService, ConsumeViewModel consume, CancellationToken cancellation = default)
    {
        return dbService.UpdateAsync<ConsumeEntity, ConsumeViewModel>(consume, cancellation);
    }
}