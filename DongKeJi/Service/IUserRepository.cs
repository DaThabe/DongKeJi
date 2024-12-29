using System.Linq.Expressions;
using DongKeJi.Common;
using DongKeJi.Common.Exceptions;
using DongKeJi.Common.Inject;
using DongKeJi.Common.Service;
using DongKeJi.Model;
using DongKeJi.Model.Entity;
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
    IApplicationContext applicationContext
) :  Repository<DongKeJiDbContext, UserEntity, UserViewModel>(services), IUserRepository
{
    public void Login(UserViewModel user, CancellationToken cancellation = default)
    {
        applicationContext.LoginUser = user;
    }

    public void Logout(UserViewModel user, CancellationToken cancellation = default)
    {
        applicationContext.LoginUser = UserViewModel.Empty;
    }

    public ValueTask AddAsync(UserViewModel user, CancellationToken cancellation = default)
    {
        return UnitOfWorkAsync(async _ =>
        {
            var userEntity = await DbContext.Users
                .FirstOrDefaultAsync(x => x.Id == user.Id, cancellation);

            if (userEntity is null || userEntity.IsEmpty())
            {
                throw new RepositoryException($"用户添加失败, 相同Id已存在\n用户信息: {user}");
            }

            //保存
            userEntity = Mapper.Map<UserEntity>(user);
            await DbContext.AddAsync(userEntity, cancellation);
            if (await DbContext.SaveChangesAsync(cancellation) <= 0)
            {
                throw new RepositoryException($"用户添加失败, 未写入数据库\n用户信息: {user}");
            }

            RegisterAutoUpdate(user);

        }, cancellation);
    }

    public ValueTask<UserViewModel> FindByNameAsync(string name, CancellationToken cancellation = default)
    {
        return UnitOfWorkAsync(async _ =>
        {
            name = name.ToLower();
            var userEntity = await DbContext.Users
                .FirstOrDefaultAsync(x => x.Name.ToLower() == name, cancellation);

            if (userEntity is null || userEntity.IsEmpty())
            {
                throw new RepositoryException($"用户查询失败, 数据不存在\n用户名称: {name}");
            }

            return RegisterAutoUpdate(userEntity);

        }, cancellation);
    }

    public async ValueTask<UserViewModel> FindByIdAsync(IIdentifiable user, CancellationToken cancellation = default)
    {
        return await UnitOfWorkAsync(async _ =>
        {
            var userEntity = await DbContext.Users
                .FirstOrDefaultAsync(x => x.Id == user.Id, cancellation);

            if (userEntity is null || userEntity.IsEmpty())
            {
                throw new RepositoryException($"用户查询失败, 数据不存在\n用户Id: {user.Id}");
            }

            return RegisterAutoUpdate(userEntity);

        }, cancellation);
    }

    public async ValueTask<IEnumerable<UserViewModel>> GetAllAsync(int? skip = null, int? take = null,
        CancellationToken cancellation = default)
    {
        return await UnitOfWorkAsync(async _ =>
        {
            var userEntityList = await DbContext.Users
                .ToListAsync(cancellation);

            return userEntityList.Select(RegisterAutoUpdate);

        }, cancellation);
    }

    public async ValueTask<IEnumerable<UserViewModel>> FindAllAsync(Expression<Func<UserEntity, bool>> queryable,
        CancellationToken cancellation = default)
    {
        return await UnitOfWorkAsync(async _ =>
        {
            var userEntityList = await DbContext.Users
                .Where(queryable)
                .ToListAsync(cancellation);

            return userEntityList.Select(RegisterAutoUpdate);

        }, cancellation);
    }
}