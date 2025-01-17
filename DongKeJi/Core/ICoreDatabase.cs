using AutoMapper;
using DongKeJi.Core.Model;
using DongKeJi.Core.Model.Entity;
using DongKeJi.Core.ViewModel.User;
using DongKeJi.Database;
using DongKeJi.Entity;
using DongKeJi.Inject;
using DongKeJi.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using UserViewModel = DongKeJi.Core.ViewModel.User.UserViewModel;

namespace DongKeJi.Core;


/// <summary>
/// 核心模块数据库
/// </summary>
public interface ICoreDatabase
{
    internal IServiceProvider ServiceProvider { get; }
}


[Inject(ServiceLifetime.Singleton, typeof(ICoreDatabase))]
internal class CoreDatabase(IServiceProvider services) : ICoreDatabase
{
    public IServiceProvider ServiceProvider => services;
}

public static class CoreDatabaseExtensions
{
    private static IAutoUpdateBuilder<TViewModel> AutoUpdate<TEntity, TViewModel>(this ICoreDatabase database, TViewModel viewModel)
        where TEntity : EntityBase
        where TViewModel : IEntityViewModel
    {
        var mapper = database.ServiceProvider.GetRequiredService<IMapper>();
        var dbContext = database.ServiceProvider.GetRequiredService<CoreDbContext>();

        return dbContext.AutoUpdate<TEntity, TViewModel>(viewModel, mapper);
    }

    public static IAutoUpdateBuilder<UserViewModel> AutoUpdate(this ICoreDatabase database, UserViewModel vm)
        => database.AutoUpdate<UserEntity, UserViewModel>(vm);
}