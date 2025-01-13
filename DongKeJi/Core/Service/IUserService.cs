using AutoMapper;
using DongKeJi.Core.Model;
using DongKeJi.Core.Model.Entity;
using DongKeJi.Core.ViewModel.User;
using DongKeJi.Database;
using DongKeJi.Exceptions;
using DongKeJi.Inject;
using DongKeJi.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DongKeJi.Core.Service;


/// <summary>
///     用户储存库
/// </summary>
public interface IUserService
{
    /// <summary>
    ///     获取"记住我"的用户Id
    /// </summary>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    ValueTask<IIdentifiable> GetRememberUserIdAsync(CancellationToken cancellation = default);

    /// <summary>
    ///     获取"记住我"的用户Id
    /// </summary>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    ValueTask ClearRememberUserIdAsync(CancellationToken cancellation = default);



    /// <summary>
    ///     登录用户
    /// </summary>
    /// <param name="user"></param>
    /// <param name="isRemember"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    ValueTask LoginAsync(UserViewModel user, bool isRemember = false, CancellationToken cancellation = default);

    /// <summary>
    ///     注销用户
    /// </summary>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    ValueTask LogoutAsync(CancellationToken cancellation = default);


    /// <summary>
    ///     创建用户
    /// </summary>
    /// <param name="value"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    ValueTask AddAsync(UserViewModel value, CancellationToken cancellation = default);

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
    /// <param name="value"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    ValueTask<UserViewModel> FindByIdAsync(IIdentifiable value, CancellationToken cancellation = default);

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

[Inject(ServiceLifetime.Singleton, typeof(IUserService))]
internal class UserService(
    IMapper mapper,
    ICoreConfigService coreConfigService,
    CoreDbContext dbContext,
    ICoreContext coreContext) :  IUserService
{
    public async ValueTask ClearRememberUserIdAsync(CancellationToken cancellation = default)
    {
        await coreConfigService.RememberUserId.SetAsync(Guid.Empty, cancellation);
    }

    public async ValueTask<IIdentifiable> GetRememberUserIdAsync(CancellationToken cancellation = default)
    {
        var guid = await coreConfigService.RememberUserId.GetAsync(cancellation: cancellation);
        return Identifiable.Create(guid);
    }

    public async ValueTask LoginAsync(UserViewModel user, bool isRemember = false, CancellationToken cancellation = default)
    {
        if (isRemember)
        {
            await coreConfigService.RememberUserId.SetAsync(user.Id, cancellation);
        }
        else
        {
            await ClearRememberUserIdAsync(cancellation);
        }

        (coreContext as CoreContext)!.CurrentUser = user;
    }

    public ValueTask LogoutAsync(CancellationToken cancellation = default)
    {
        (coreContext as CoreContext)!.CurrentUser = null;
        return ValueTask.CompletedTask;
    }

    public async ValueTask AddAsync(UserViewModel value, CancellationToken cancellation = default)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellation);

        try
        {
            //验证
            value.AssertValidate();

            //查询
            var userEntity = await dbContext.Users
                .FirstOrDefaultAsync(x => x.Id == value.Id, cancellation);
             DatabaseException.ThrowIfEntityAlreadyExists(userEntity, "用户已存在");

            //保存
            userEntity = mapper.Map<UserEntity>(value);
            await dbContext.AddAsync(userEntity, cancellation);

            await dbContext.AssertSaveSuccessAsync(cancellation: cancellation);
            await transaction.CommitAsync(cancellation);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellation);
            throw new DatabaseException($"添加用户时发生错误\n用户信息: {value}", ex);
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
            userEntity = DatabaseException.ThrowIfEntityNotFound(userEntity, "用户不存在");

            return mapper.Map<UserViewModel>(userEntity);
        }
        catch (Exception ex)
        {
            //await transaction.RollbackAsync(cancellation);
            throw new DatabaseException($"查询用户时发生错误\n用户名称: {name}", ex);
        }
    }

    public async ValueTask<UserViewModel> FindByIdAsync(IIdentifiable value, CancellationToken cancellation = default)
    {
        //await using var transaction = await DbContext.Database.BeginTransactionAsync(cancellation);

        try
        {
            var userEntity = await dbContext.Users
                .FirstOrDefaultAsync(x => x.Id == value.Id, cancellation);
            userEntity = DatabaseException.ThrowIfEntityNotFound(userEntity, "用户不存在");

            return mapper.Map<UserViewModel>(userEntity);
        }
        catch (Exception ex)
        {
            //await transaction.RollbackAsync(cancellation);
            throw new DatabaseException($"查询用户时发生错误\n用户Id: {value.Id}", ex);
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

            return userEntityList.Select(mapper.Map<UserViewModel>);
        }
        catch (Exception ex)
        {
            //await transaction.RollbackAsync(cancellation);
            throw new DatabaseException($"获取所有用户时发生错误", ex);
        }
    }

    public ValueTask UpdateAsync(UserViewModel viewModel)
    {
        throw new NotImplementedException();
    }
}