using AutoMapper;
using DongKeJi.Core.Service;
using DongKeJi.Core.ViewModel.User;
using DongKeJi.Database;
using DongKeJi.Exceptions;
using DongKeJi.Extensions;
using DongKeJi.Inject;
using DongKeJi.Validation;
using DongKeJi.Work.Model;
using DongKeJi.Work.Model.Entity.Staff;
using DongKeJi.Work.ViewModel.Staff;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DongKeJi.Work.Service;

/// <summary>
/// 员工服务
/// </summary>
public interface IStaffService
{
    /// <summary>
    /// 将指定员工设置为用户的主员工
    /// </summary>
    /// <param name="staff"></param>
    /// <param name="user"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    ValueTask SetPrimaryToUserAsync(
        IIdentifiable staff, 
        IIdentifiable user, 
        CancellationToken cancellation = default);

    /// <summary>
    /// 获取主员工
    /// </summary>
    /// <param name="user"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    ValueTask<IIdentifiable> GetPrimaryIdAsync(
        IIdentifiable user, 
        CancellationToken cancellation = default);

    /// <summary>
    /// 设置指定员工为活跃状态  (办公环境下的默认员工
    /// </summary>
    /// <param name="staff"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    ValueTask SetPrimaryAsync(
        IIdentifiable staff,
        CancellationToken cancellation = default);


    /// <summary>
    ///     根据员工获取用户
    /// </summary>
    /// <param name="staff"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    ValueTask<UserViewModel> GetUserByStaffAsync(
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
    ///     根据Id获取员工
    /// </summary>
    /// <param name="staff"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    ValueTask<StaffViewModel> GetByIdAsync(
        IIdentifiable staff,
        CancellationToken cancellation = default);

    /// <summary>
    ///     根据用户获取所有员工
    /// </summary>
    /// <param name="user"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    ValueTask<IEnumerable<StaffViewModel>> GetAllByUserAsync(
        IIdentifiable user,
        CancellationToken cancellation = default);

    /// <summary>
    ///     获取某个职位类型的所有员工
    /// </summary>
    /// <param name="type"></param>
    /// <param name="take"></param>
    /// <param name="cancellation"></param>
    /// <param name="skip"></param>
    /// <returns></returns>
    ValueTask<IEnumerable<StaffViewModel>> GetAllByPositionTypeAsync(
        StaffPositionType type, 
        int? skip = null,
        int? take = null, 
        CancellationToken cancellation = default);

    /// <summary>
    ///     获取符合职位和用户的所有员工
    /// </summary>
    /// <param name="user"></param>
    /// <param name="type"></param>
    /// <param name="take"></param>
    /// <param name="cancellation"></param>
    /// <param name="skip"></param>
    /// <returns></returns>
    ValueTask<IEnumerable<StaffViewModel>> GetAllByUserAndPositionTypeAsync(
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
/// 员工服务默认实现
/// </summary>
[Inject(ServiceLifetime.Singleton, typeof(IStaffService))]
internal class StaffService(
    IUserService userService,
    IMapper mapper,
    IWorkModule workModule,
    IWorkConfig workConfig,
    WorkDbContext dbContext) : IStaffService
{

    public async ValueTask SetPrimaryToUserAsync(
        IIdentifiable staff,
        IIdentifiable user,
        CancellationToken cancellation = default)
    {
        try
        {
            var ps = await workConfig.PrimaryStaff.GetAsync(cancellation: cancellation);
            ps[user.Id] = staff.Id;
            await workConfig.PrimaryStaff.UpdateAsync(cancellation);
        }
        catch
        {
            try
            {
                await workConfig.PrimaryStaff.SetAsync(new()
                {
                    { user.Id, staff.Id }

                }, cancellation);
            }
            catch (Exception ex)
            {
                throw new ConfigException($"绑定用户主员工时发生错误\n用户Id: {user.Id}\n员工Id: {staff.Id}", ex);
            }
        }
    }

    public async ValueTask<IIdentifiable> GetPrimaryIdAsync(
        IIdentifiable user,
        CancellationToken cancellation = default)
    {
        try
        {
            var ps = await workConfig.PrimaryStaff.GetAsync(cancellation: cancellation);

            if (ps.TryGetValue(user.Id, out var staffId))
            {
                return Identifiable.Create(staffId);
            }

            throw new ArgumentNullException(nameof(user), "用户不存在或未关联员工");
        }
        catch (Exception ex)
        {
            throw new ConfigException($"获取用户绑定主员工时发生错误\n用户Id: {user.Id}", ex);
        }
    }

    public async ValueTask SetPrimaryAsync(
        IIdentifiable staff,
        CancellationToken cancellation = default)
    {
        var staffVm = await GetByIdAsync(staff, cancellation);
        (workModule as WorkModule)!.CurrentStaff = staffVm;
    }

    public async ValueTask<UserViewModel> GetUserByStaffAsync(
        IIdentifiable staff,
        CancellationToken cancellation = default)
    {
        //await using var transaction = await DbContext.Database.BeginTransactionAsync(cancellation);

        try
        {
            //员工
            var staffEntity = await dbContext.Staff
                .FirstOrDefaultAsync(x => x.Id == staff.Id, cancellation);
            staffEntity = DatabaseException.ThrowIfEntityNotFound(staffEntity, "员工不存在");

            //用户Id
            if (staffEntity.UserId is null) throw new DatabaseException("关联用户为空");

            //用户
            Identifiable id = staffEntity.UserId;
            var userViewModel = await userService.GetByIdAsync(id, cancellation);
            userViewModel = DatabaseException.ThrowIfEntityNotFound(userViewModel, "用户不存在");

            return userViewModel;
        }
        catch (Exception ex)
        {
            //await transaction.RollbackAsync(cancellation);
            throw new DatabaseException($"获取员工关联用户时发生错误\n员工信息: {staff}", ex);
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
            staff.AssertValidate();

            //员工
            var staffEntity = await dbContext.Staff
                .FirstOrDefaultAsync(x => x.Id == staff.Id, cancellation);
            DatabaseException.ThrowIfEntityAlreadyExists(staffEntity, "员工已存在");

            //用户
            var userViewModel = await userService.GetByIdAsync(user, cancellation);
            userViewModel = DatabaseException.ThrowIfEntityNotFound(userViewModel, "用户不存在");

            //修改
            staffEntity = mapper.Map<StaffEntity>(staff);
            staffEntity.UserId = userViewModel.Id;
            dbContext.Add(staffEntity);

            //保存
            await dbContext.AssertSaveChangesAsync(cancellation);
            await transaction.CommitAsync(cancellation);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellation);
            throw new DatabaseException($"添加员工时发生错误\n员工信息: {staff}\n用户Id: {user.Id}", ex);
        }
    }

    public async ValueTask<StaffViewModel> GetByIdAsync(
        IIdentifiable staff,
        CancellationToken cancellation = default)
    {
        //await using var transaction = await DbContext.Database.BeginTransactionAsync(cancellation);

        try
        {
            var staffEntity = await dbContext.Staff
                .FirstOrDefaultAsync(x => x.Id == staff.Id, cancellation);
            staffEntity = DatabaseException.ThrowIfEntityNotFound(staffEntity, "员工不存在");

            return mapper.Map<StaffViewModel>(staffEntity);
        }
        catch (Exception ex)
        {
            //await transaction.RollbackAsync(cancellation);
            throw new DatabaseException($"获取员工时发生错误\n员工Id: {staff.Id}", ex);
        }
    }

    public async ValueTask<IEnumerable<StaffViewModel>> GetAllByUserAsync(
        IIdentifiable user,
        CancellationToken cancellation = default)
    {
        //await using var transaction = await DbContext.Database.BeginTransactionAsync(cancellation);

        try
        {
            var staffEntityList = await dbContext.Staff
                .Where(x => x.UserId == user.Id)
                .ToListAsync(cancellation);

            return staffEntityList.Select(mapper.Map<StaffViewModel>);
        }
        catch (Exception ex)
        {
            //await transaction.RollbackAsync(cancellation);
            throw new DatabaseException($"根据用户获取所有员工时发生错误\n用户Id: {user}", ex);
        }
    }

    public async ValueTask<IEnumerable<StaffViewModel>> GetAllByPositionTypeAsync(
        StaffPositionType type,
        int? skip = null,
        int? take = null,
        CancellationToken cancellation = default)
    {
        //await using var transaction = await DbContext.Database.BeginTransactionAsync(cancellation);

        try
        {
            var staffPositionEntity = await dbContext.StaffPosition
                .Include(x => x.Staffs)
                .Where(x => x.Type == type)
                .Select(x => x.Staffs
                    .OrderBy(staff => staff.Id)
                    .SkipAndTake(skip, take)
                    .ToList())
                .FirstOrDefaultAsync(cancellation) ?? [];

            return staffPositionEntity.Select(mapper.Map<StaffViewModel>);
        }
        catch (Exception ex)
        {
            //await transaction.RollbackAsync(cancellation);
            throw new DatabaseException($"根据职位获取所有员工时发生错误\n职位类型: {type}\nSkip: {skip}\nTake: {take}", ex);
        }
    }

    public async ValueTask<IEnumerable<StaffViewModel>> GetAllByUserAndPositionTypeAsync(
        IIdentifiable user,
        StaffPositionType type,
        int? skip = null,
        int? take = null,
        CancellationToken cancellation = default)
    {
        //await using var transaction = await DbContext.Database.BeginTransactionAsync(cancellation);

        try
        {
            var positionEntity = await dbContext.StaffPosition
                .Include(x => x.Staffs)
                .Where(x => x.Type == type)
                .FirstOrDefaultAsync(cancellation);

            if (positionEntity is null || positionEntity.IsNullOrEmpty()) return [];

            var staffEntityList = await dbContext.Staff
                .Include(x => x.Positions)
                .Where(x => x.UserId == user.Id)
                .Where(x => x.Positions.Contains(positionEntity))
                .SkipAndTake(skip, take)
                .ToListAsync(cancellation);

            return staffEntityList.Select(mapper.Map<StaffViewModel>);
        }
        catch (Exception ex)
        {
            //await transaction.RollbackAsync(cancellation);
            throw new DatabaseException($"根据职位和用户获取所有员工时发生错误\n用户Id: {user.Id}\n职位类型: {type}\nSkip: {skip}\nTake: {take}", ex);
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
            var staffEntityList = await dbContext.Staff
                .SkipAndTake(skip, take)
                .ToListAsync(cancellation);

            return staffEntityList.Select(mapper.Map<StaffViewModel>);
        }
        catch (Exception ex)
        {
            //await transaction.RollbackAsync(cancellation);
            throw new DatabaseException($"获取所有员工时发生错误\nSkip: {skip}\nTake: {take}", ex);
        }
    }
}