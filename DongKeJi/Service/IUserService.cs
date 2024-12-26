using System.Linq.Expressions;
using DongKeJi.Common;
using DongKeJi.Common.Database;
using DongKeJi.Common.Inject;
using DongKeJi.Model;
using DongKeJi.Model.Entity;
using DongKeJi.ViewModel.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using AutoMapper;
using DongKeJi.ViewModel;

namespace DongKeJi.Service;


/// <summary>
/// 用户服务
/// </summary>
public interface IUserService
{
    /// <summary>
    /// 登录用户
    /// </summary>
    /// <param name="user"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    ValueTask<bool> LoginAsync(UserViewModel user, CancellationToken cancellation = default);

    /// <summary>
    /// 注销用户
    /// </summary>
    /// <param name="user"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    ValueTask<bool> LogoutAsync(UserViewModel user, CancellationToken cancellation = default);


    /// <summary>
    /// 创建用户
    /// </summary>
    /// <param name="user"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    ValueTask<UserViewModel?> CreateAsync(UserViewModel user, CancellationToken cancellation = default);

    /// <summary>
    /// 根据用户名查询
    /// </summary>
    /// <param name="name"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    ValueTask<UserViewModel?> FindByNameAsync(string name, CancellationToken cancellation = default);

    /// <summary>
    /// 根据用户Id查询
    /// </summary>
    /// <param name="user"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    ValueTask<UserViewModel?> FindByIdAsync(IIdentifiable user, CancellationToken cancellation = default);

    /// <summary>
    /// 获取所有用户
    /// </summary>
    /// <param name="take"></param>
    /// <param name="cancellation"></param>
    /// <param name="skip"></param>
    /// <returns></returns>
    ValueTask<IEnumerable<UserViewModel>> GetAllAsync(int? skip = null, int? take = null, CancellationToken cancellation = default);
}


[Inject(ServiceLifetime.Singleton, typeof(IUserService))]
internal class UserService(
    DongKeJiDbContext dbContext,
    IApplicationContext applicationContext,
    IMapper mapper,
    ILogger<UserService> logger
    ) : IUserService
{
    public ValueTask<bool> LoginAsync(UserViewModel user, CancellationToken cancellation = default)
    {
        applicationContext.User = user;
        return ValueTask.FromResult(true);
    }

    public ValueTask<bool> LogoutAsync(UserViewModel user, CancellationToken cancellation = default)
    {
        applicationContext.User = UserViewModel.Empty;
        return ValueTask.FromResult(true);
    }

    public async ValueTask<UserViewModel?> CreateAsync(UserViewModel user, CancellationToken cancellation = default)
    {
        return await dbContext.UnitOfWorkAsync(async _ =>
        {
            var userEntity = await dbContext.Users
                .FirstOrDefaultAsync(x => x.Id == user.Id, cancellationToken: cancellation);

            if (userEntity is not null)
            {
                throw new Exception("用户已存在");
            }

            //修改
            userEntity = mapper.Map<UserEntity>(user);
            await dbContext.AddAsync(userEntity, cancellation);

            //保存
            await dbContext.SaveChangesAsync(cancellation);
            return RegisterAutoUpdate(userEntity);

        }, ex => logger.LogError(ex, "添加用户时发生错误, 用户Id:{user}", user.Id));
    }

    public async ValueTask<UserViewModel?> FindByNameAsync(string name, CancellationToken cancellation = default)
    {
        return await dbContext.UnitOfWorkAsync(async _ =>
        {
            name = name.ToLower();
            var userEntity = await dbContext.Users
                .FirstOrDefaultAsync(x => x.Name.ToLower() == name, cancellationToken: cancellation);

            if (userEntity is null)
            {
                logger.LogWarning("根据用户名查询用户时发生错误, 用户不存在, 用户名:{user}", name);
                return null;
            }

            return RegisterAutoUpdate(userEntity);

        }, ex => logger.LogError(ex, "根据用户名查询用户时发生错误, 用户名:{user}", name));
    }

    public async ValueTask<UserViewModel?> FindByIdAsync(IIdentifiable user, CancellationToken cancellation = default)
    {
        return await dbContext.UnitOfWorkAsync(async _ =>
        {
            var userEntity = await dbContext.Users
                .FirstOrDefaultAsync(x => x.Id == user.Id, cancellationToken: cancellation);

            if (userEntity is null)
            {
                logger.LogWarning("根据用户名查询用户时发生错误, 用户不存在, 用户Id:{user}", user.Id);
                return null;
            }

            return RegisterAutoUpdate(userEntity);

        }, ex => logger.LogError(ex, "根据用户名查询用户时发生错误, 用户Id:{user}", user.Id));
    }

    public async ValueTask<IEnumerable<UserViewModel>> FindAllAsync(Expression<Func<UserEntity, bool>> queryable, CancellationToken cancellation = default)
    {
        return await dbContext.UnitOfWorkAsync(async _ =>
        {
            var userEntityList = await dbContext.Users
                .Where(queryable)
                .ToListAsync(cancellation);

            return userEntityList.Select(RegisterAutoUpdate);

        }, ex => logger.LogError(ex, "自定义查询所有用户时发生错误, 表达式:{queryable}", queryable)) ?? [];
    }

    public async ValueTask<IEnumerable<UserViewModel>> GetAllAsync(int? skip = null, int? take = null, CancellationToken cancellation = default)
    {
        return await dbContext.UnitOfWorkAsync(async _ =>
        {
            var userEntityList = await dbContext.Users
                .ToListAsync(cancellation);

            return userEntityList.Select(RegisterAutoUpdate);

        }, ex => logger.LogError(ex, "获取所有用户时发生错误")) ?? [];
    }




    protected UserViewModel RegisterAutoUpdate(UserEntity entity)
    {
        var vm = mapper.Map<UserViewModel>(entity);
        return RegisterAutoUpdate(vm);
    }

    protected UserViewModel RegisterAutoUpdate(UserViewModel vm)
    {
        vm.PropertyChanged += async (_, _) =>
        {
            var existEntity = await dbContext.Users
                .FirstOrDefaultAsync(x => x.Id == vm.Id);

            if (existEntity is null)
            {
                logger.LogError("自动更新失败, 数据不存在, 类型:{type} Id:{id}", vm.GetType(), vm.Id);
                return;
            }

            //修改
            mapper.Map(vm, existEntity);

            //保存
            var result = await dbContext.SaveChangesAsync();
            if (result <= 0) logger.LogWarning("自动更新失败, 未写入内容, 类型:{type} Id:{id}", vm.GetType(), vm.Id);
        };

        return vm;
    }
}
