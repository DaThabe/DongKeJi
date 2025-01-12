using AutoMapper;
using DongKeJi.Core.Model;
using DongKeJi.Core.Model.Entity;
using DongKeJi.Core.ViewModel.User;
using DongKeJi.Database;
using DongKeJi.Entity;
using DongKeJi.Inject;
using DongKeJi.ViewModel;
using Microsoft.Extensions.DependencyInjection;

namespace DongKeJi.Core.Service;

public interface ICoreDbService
{
    internal IServiceProvider ServiceProvider { get; }
}


[Inject(ServiceLifetime.Singleton, typeof(ICoreDbService))]
internal class CoreDatabase(IServiceProvider services) : ICoreDbService
{
    public IServiceProvider ServiceProvider => services;
}


public static class CoreDatabaseExtensions
{
    private static IAutoUpdateBuilder<TViewModel> AutoUpdate<TEntity, TViewModel>(this ICoreDbService dbService, TViewModel viewModel)
        where TEntity : EntityBase
        where TViewModel : IEntityViewModel
    {
        var mapper = dbService.ServiceProvider.GetRequiredService<IMapper>();
        var dbContext = dbService.ServiceProvider.GetRequiredService<CoreDbContext>();

        return dbContext.AutoUpdate<TEntity, TViewModel>(viewModel, mapper);
    }

    public static IAutoUpdateBuilder<UserViewModel> AutoUpdate(this ICoreDbService dbService, UserViewModel vm)
        => dbService.AutoUpdate<UserEntity, UserViewModel>(vm);
}