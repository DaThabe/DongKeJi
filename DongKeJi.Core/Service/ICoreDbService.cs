using AutoMapper;
using DongKeJi.Core.Model;
using DongKeJi.Core.Model.Entity;
using DongKeJi.Core.ViewModel.User;
using DongKeJi.Database;
using DongKeJi.Inject;
using Microsoft.Extensions.DependencyInjection;

namespace DongKeJi.Core.Service;

public interface ICoreDbService
{
    internal CoreDbContext DbContext { get; }

    internal IMapper Mapper { get; }
}

[Inject(ServiceLifetime.Singleton, typeof(ICoreDbService))]
internal class CoreDatabase(CoreDbContext dbContext, IMapper mapper) : ICoreDbService
{
    public CoreDbContext DbContext => dbContext;
    public IMapper Mapper => mapper;
}


public static class CoreDatabaseExtensions
{
    public static IAutoUpdateBuilder<UserViewModel> RegisterAutoUpdate(this ICoreDbService dbService, UserViewModel user)
    {
        return new AutoUpdateBuilder<UserEntity, UserViewModel>(dbService.DbContext, dbService.Mapper, user);
    }
}