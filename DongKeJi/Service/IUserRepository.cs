using AutoMapper;
using DongKeJi.Common;
using DongKeJi.Common.Exceptions;
using DongKeJi.Common.Inject;
using DongKeJi.Common.Service;
using DongKeJi.Common.Validation;
using DongKeJi.Model;
using DongKeJi.Model.Entity;
using DongKeJi.Model.Validation;
using DongKeJi.ViewModel;
using DongKeJi.ViewModel.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;


namespace DongKeJi.Service;


/// <summary>
///     用户储存库
/// </summary>
public interface IUserRepository
{
    /// <summary>
    ///     登录用户
    /// </summary>
    /// <param name="user"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    void Login(UserViewModel user, CancellationToken cancellation = default);

    /// <summary>
    ///     注销用户
    /// </summary>
    /// <param name="user"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    void Logout(UserViewModel user, CancellationToken cancellation = default);


    /// <summary>
    ///     创建用户
    /// </summary>
    /// <param name="user"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    ValueTask AddAsync(UserViewModel user, CancellationToken cancellation = default);

    /// <summary>
    ///     根据用户名查询
    /// </summary>
    /// <param name="name"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    ValueTask<UserViewModel> FindByNameAsync(string name, CancellationToken cancellation = default);

    /// <summary>
    ///     根据用户Id查询
    /// </summary>
    /// <param name="user"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    ValueTask<UserViewModel> FindByIdAsync(IIdentifiable user, CancellationToken cancellation = default);

    /// <summary>
    ///     获取所有用户
    /// </summary>
    /// <param name="take"></param>
    /// <param name="cancellation"></param>
    /// <param name="skip"></param>
    /// <returns></returns>
    ValueTask<IEnumerable<UserViewModel>> GetAllAsync(int? skip = null, int? take = null,
        CancellationToken cancellation = default);
}

[Inject(ServiceLifetime.Singleton, typeof(IUserRepository))]
internal class UserRepository(
    IServiceProvider services,
    IMapper mapper,
    UserViewModelValidationSet validation,
    CoreDbContext dbContext,
    IApplicationContext applicationContext) :  IUserRepository
{
    public void Login(UserViewModel user, CancellationToken cancellation = default)
    {
        applicationContext.LoginUser = user;
    }

    public void Logout(UserViewModel user, CancellationToken cancellation = default)
    {
        applicationContext.LoginUser = UserViewModel.Empty;
    }

    public async ValueTask AddAsync(UserViewModel user, CancellationToken cancellation = default)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellation);

        try
        {
            //验证
            validation.AssertValidate(user);

            //查询
            var userEntity = await dbContext.Users
                .FirstOrDefaultAsync(x => x.Id == user.Id, cancellation);
             RepositoryException.ThrowIfEntityAlreadyExists(userEntity, "用户已存在");

            //保存
            userEntity = mapper.Map<UserEntity>(user);
            await dbContext.AddAsync(userEntity, cancellation);

            await dbContext.AssertSaveSuccessAsync(cancellation: cancellation);
            await transaction.CommitAsync(cancellation);

            dbContext.RegisterAutoUpdate<UserEntity, UserViewModel>(user, services);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellation);
            throw new RepositoryException($"添加用户时发生错误\n用户信息: {user}", ex);
        }
    }

    public async ValueTask<UserViewModel> FindByNameAsync(string name, CancellationToken cancellation = default)
    {
        //await using var transaction = await DbContext.Database.BeginTransactionAsync(cancellation);

        try
        {
            var searchName = name.ToLower();
            var userEntity = await dbContext.Users
                .FirstOrDefaultAsync(x => x.Name.ToLower() == searchName, cancellation);
            userEntity = RepositoryException.ThrowIfEntityNotFound(userEntity, "用户不存在");

            return dbContext.RegisterAutoUpdate<UserEntity, UserViewModel>(userEntity, services);
        }
        catch (Exception ex)
        {
            //await transaction.RollbackAsync(cancellation);
            throw new RepositoryException($"查询用户时发生错误\n用户名称: {name}", ex);
        }
    }

    public async ValueTask<UserViewModel> FindByIdAsync(IIdentifiable user, CancellationToken cancellation = default)
    {
        //await using var transaction = await DbContext.Database.BeginTransactionAsync(cancellation);

        try
        {
            var userEntity = await dbContext.Users
                .FirstOrDefaultAsync(x => x.Id == user.Id, cancellation);
            userEntity = RepositoryException.ThrowIfEntityNotFound(userEntity, "用户不存在");

            return dbContext.RegisterAutoUpdate<UserEntity, UserViewModel>(userEntity, services);
        }
        catch (Exception ex)
        {
            //await transaction.RollbackAsync(cancellation);
            throw new RepositoryException($"查询用户时发生错误\n用户Id: {user.Id}", ex);
        }
    }

    public async ValueTask<IEnumerable<UserViewModel>> GetAllAsync(int? skip = null, int? take = null,
        CancellationToken cancellation = default)
    {
        //await using var transaction = await DbContext.Database.BeginTransactionAsync(cancellation);

        try
        {
            var userEntityList = await dbContext.Users
                .ToListAsync(cancellation);

            return userEntityList.Select(x => dbContext.RegisterAutoUpdate<UserEntity, UserViewModel>(x, services));
        }
        catch (Exception ex)
        {
            //await transaction.RollbackAsync(cancellation);
            throw new RepositoryException($"获取所有用户时发生错误", ex);
        }
    }
}