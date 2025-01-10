using AutoMapper;
using DongKeJi.Common;
using DongKeJi.Common.Exceptions;
using DongKeJi.Common.Extensions;
using DongKeJi.Common.Inject;
using DongKeJi.Common.Service;
using DongKeJi.Common.Validation;
using DongKeJi.Model;
using DongKeJi.Model.Entity;
using DongKeJi.ViewModel.User;
using DongKeJi.Work.Model;
using DongKeJi.Work.Model.Entity.Staff;
using DongKeJi.Work.ViewModel.Common.Staff;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DongKeJi.Work.Service;

/// <summary>
///     员工出储存库
/// </summary>
public interface IStaffRepository
{
    /// <summary>
    ///     根据员工查询用户
    /// </summary>
    /// <param name="staff"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    ValueTask<UserViewModel> FindUserByStaffAsync(
        IIdentifiable staff, 
        CancellationToken cancellation = default);

    /// <summary>
    ///     添加员工
    /// </summary>
    /// <param name="staff"></param>
    /// <param name="user"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    ValueTask AddAsync(
        StaffViewModel staff, 
        IIdentifiable user, 
        CancellationToken cancellation = default);

    /// <summary>
    ///     根据Id查询员工
    /// </summary>
    /// <param name="staff"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    ValueTask<StaffViewModel> FindByIdAsync(
        IIdentifiable staff,
        CancellationToken cancellation = default);

    /// <summary>
    ///     根据用户查询所有员工
    /// </summary>
    /// <param name="user"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    ValueTask<IEnumerable<StaffViewModel>> FindAllByUserAsync(
        IIdentifiable user,
        CancellationToken cancellation = default);

    /// <summary>
    ///     查询某个职位类型的所有员工
    /// </summary>
    /// <param name="type"></param>
    /// <param name="take"></param>
    /// <param name="cancellation"></param>
    /// <param name="skip"></param>
    /// <returns></returns>
    ValueTask<IEnumerable<StaffViewModel>> FindAllByPositionTypeAsync(
        StaffPositionType type, 
        int? skip = null,
        int? take = null, 
        CancellationToken cancellation = default);

    /// <summary>
    ///     查询符合职位和用户的所有员工
    /// </summary>
    /// <param name="user"></param>
    /// <param name="type"></param>
    /// <param name="take"></param>
    /// <param name="cancellation"></param>
    /// <param name="skip"></param>
    /// <returns></returns>
    ValueTask<IEnumerable<StaffViewModel>> FindAllByUserAndPositionTypeAsync(
        IIdentifiable user, 
        StaffPositionType type,
        int? skip = null,
        int? take = null, 
        CancellationToken cancellation = default);

    /// <summary>
    ///     获取所有员工
    /// </summary>
    /// <param name="take"></param>
    /// <param name="cancellation"></param>
    /// <param name="skip"></param>
    /// <returns></returns>
    ValueTask<IEnumerable<StaffViewModel>> GetAllAsync(
        int? skip = null, 
        int? take = null,
        CancellationToken cancellation = default);
}

/// <summary>
///     员工服务
/// </summary>
[Inject(ServiceLifetime.Singleton, typeof(IStaffRepository))]
internal class StaffRepository(
    IServiceProvider services,
    IMapper mapper,
    IValidation<StaffViewModel> validation,
    CoreDbContext coreDbContext,
    WorkDbContext dbContext) : IStaffRepository
{
    public async ValueTask<UserViewModel> FindUserByStaffAsync(
        IIdentifiable staff,
        CancellationToken cancellation = default)
    {
        //await using var transaction = await DbContext.Database.BeginTransactionAsync(cancellation);

        try
        {
            //员工
            var staffEntity = await dbContext.Staffs
                .FirstOrDefaultAsync(x => x.Id == staff.Id, cancellation);
            staffEntity = RepositoryException.ThrowIfEntityNotFound(staffEntity, "员工不存在");

            //用户Id
            if (staffEntity.UserId is null) throw new RepositoryException("关联用户为空");

            //用户
            var userEntity = await coreDbContext.Users
                .FirstOrDefaultAsync(x => x.Id == staffEntity.UserId, cancellation);
            userEntity = RepositoryException.ThrowIfEntityNotFound(userEntity, "用户不存在");

            return coreDbContext.RegisterAutoUpdate<UserEntity, UserViewModel>(userEntity, services);
        }
        catch (Exception ex)
        {
            //await transaction.RollbackAsync(cancellation);
            throw new RepositoryException($"查询员工关联用户时发生错误\n员工信息: {staff}", ex);
        }
    }

    public async ValueTask AddAsync(
        StaffViewModel staff,
        IIdentifiable user,
        CancellationToken cancellation = default)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellation);

        try
        {
            //验证
            validation.AssertValidate(staff);

            //员工
            var staffEntity = await dbContext.Staffs
                .FirstOrDefaultAsync(x => x.Id == staff.Id, cancellation);
            RepositoryException.ThrowIfEntityAlreadyExists(staffEntity, "员工已存在");

            //用户
            var userEntity = await coreDbContext.Users
                .FirstOrDefaultAsync(x => x.Id == user.Id, cancellation);
            userEntity = RepositoryException.ThrowIfEntityNotFound(userEntity, "用户不存在");

            //修改
            staffEntity = mapper.Map<StaffEntity>(staff);
            staffEntity.UserId = userEntity.Id;
            dbContext.Add(staffEntity);

            //保存
            await dbContext.AssertSaveSuccessAsync(cancellation);
            await transaction.CommitAsync(cancellation);

            dbContext.RegisterAutoUpdate<StaffEntity, StaffViewModel>(staff, services);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellation);
            throw new RepositoryException($"添加员工时发生错误\n员工信息: {staff}\n用户Id: {user.Id}", ex);
        }
    }

    public async ValueTask<StaffViewModel> FindByIdAsync(
        IIdentifiable staff,
        CancellationToken cancellation = default)
    {
        //await using var transaction = await DbContext.Database.BeginTransactionAsync(cancellation);

        try
        {
            var staffEntity = await dbContext.Staffs
                .FirstOrDefaultAsync(x => x.Id == staff.Id, cancellation);
            staffEntity = RepositoryException.ThrowIfEntityNotFound(staffEntity, "员工不存在");

            return dbContext.RegisterAutoUpdate<StaffEntity, StaffViewModel>(staffEntity, services);
        }
        catch (Exception ex)
        {
            //await transaction.RollbackAsync(cancellation);
            throw new RepositoryException($"查询员工时发生错误\n员工Id: {staff.Id}", ex);
        }
    }

    public async ValueTask<IEnumerable<StaffViewModel>> FindAllByUserAsync(
        IIdentifiable user,
        CancellationToken cancellation = default)
    {
        //await using var transaction = await DbContext.Database.BeginTransactionAsync(cancellation);

        try
        {
            var staffEntityList = await dbContext.Staffs
                .Where(x => x.UserId == user.Id)
                .ToListAsync(cancellation);

            return staffEntityList.Select(x => dbContext.RegisterAutoUpdate<StaffEntity, StaffViewModel>(x, services));
        }
        catch (Exception ex)
        {
            //await transaction.RollbackAsync(cancellation);
            throw new RepositoryException($"根据用户查询所有员工时发生错误\n用户Id: {user}", ex);
        }
    }

    public async ValueTask<IEnumerable<StaffViewModel>> FindAllByPositionTypeAsync(
        StaffPositionType type,
        int? skip = null,
        int? take = null,
        CancellationToken cancellation = default)
    {
        //await using var transaction = await DbContext.Database.BeginTransactionAsync(cancellation);

        try
        {
            var staffPositionEntity = await dbContext.StaffPositions
                .Include(x => x.Staffs)
                .Where(x => x.Type == type)
                .Select(x => x.Staffs
                    .OrderBy(staff => staff.Id)
                    .SkipAndTake(skip, take)
                    .ToList())
                .FirstOrDefaultAsync(cancellation) ?? [];

            return staffPositionEntity.Select(x =>
                dbContext.RegisterAutoUpdate<StaffEntity, StaffViewModel>(x, services));
        }
        catch (Exception ex)
        {
            //await transaction.RollbackAsync(cancellation);
            throw new RepositoryException($"根据职位查询所有员工时发生错误\n职位类型: {type}", ex);
        }
    }

    public async ValueTask<IEnumerable<StaffViewModel>> FindAllByUserAndPositionTypeAsync(
        IIdentifiable user,
        StaffPositionType type,
        int? skip = null,
        int? take = null,
        CancellationToken cancellation = default)
    {
        //await using var transaction = await DbContext.Database.BeginTransactionAsync(cancellation);

        try
        {
            var positionEntity = await dbContext.StaffPositions
                .Include(x => x.Staffs)
                .Where(x => x.Type == type)
                .FirstOrDefaultAsync(cancellation);

            if (positionEntity is null || positionEntity.IsEmpty()) return [];

            var staffEntityList = await dbContext.Staffs
                .Include(x => x.Positions)
                .Where(x => x.UserId == user.Id)
                .Where(x => !x.IsPrimaryAccount)
                .Where(x => x.Positions.Contains(positionEntity))
                .SkipAndTake(skip, take)
                .ToListAsync(cancellation);

            return staffEntityList.Select(x => dbContext.RegisterAutoUpdate<StaffEntity, StaffViewModel>(x, services));
        }
        catch (Exception ex)
        {
            //await transaction.RollbackAsync(cancellation);
            throw new RepositoryException($"根据职位和用户查询所有员工时发生错误\n用户Id: {user.Id}\n职位类型: {type}", ex);
        }
    }

    public async ValueTask<IEnumerable<StaffViewModel>> GetAllAsync(
        int? skip = null,
        int? take = null,
        CancellationToken cancellation = default)
    {
        //await using var transaction = await DbContext.Database.BeginTransactionAsync(cancellation);

        try
        {
            var staffEntityList = await dbContext.Staffs
                .SkipAndTake(skip, take)
                .ToListAsync(cancellation);

            return staffEntityList.Select(x => dbContext.RegisterAutoUpdate<StaffEntity, StaffViewModel>(x, services));
        }
        catch (Exception ex)
        {
            //await transaction.RollbackAsync(cancellation);
            throw new RepositoryException("获取所有员工时发生错误", ex);
        }
    }
}