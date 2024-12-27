using AutoMapper;
using DongKeJi.Common;
using DongKeJi.Common.Exceptions;
using DongKeJi.Common.Extensions;
using DongKeJi.Common.Inject;
using DongKeJi.Common.Service;
using DongKeJi.Model;
using DongKeJi.Service;
using DongKeJi.ViewModel.User;
using DongKeJi.Work.Model;
using DongKeJi.Work.Model.Entity.Staff;
using DongKeJi.Work.ViewModel.Common.Staff;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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
    ValueTask<UserViewModel> FindUserByStaffAsync(IIdentifiable staff, CancellationToken cancellation = default);

    /// <summary>
    ///     添加员工
    /// </summary>
    /// <param name="staff"></param>
    /// <param name="user"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    ValueTask AddAsync(StaffViewModel staff, IIdentifiable user, CancellationToken cancellation = default);

    /// <summary>
    ///     根据用户查询所有员工
    /// </summary>
    /// <param name="user"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    ValueTask<IEnumerable<StaffViewModel>> FindAllByUserAsync(IIdentifiable user,
        CancellationToken cancellation = default);

    /// <summary>
    ///     查询某个职位类型的所有员工
    /// </summary>
    /// <param name="type"></param>
    /// <param name="take"></param>
    /// <param name="cancellation"></param>
    /// <param name="skip"></param>
    /// <returns></returns>
    ValueTask<IEnumerable<StaffViewModel>> FindAllByPositionTypeAsync(StaffPositionType type, int? skip = null,
        int? take = null, CancellationToken cancellation = default);

    /// <summary>
    ///     查询符合职位和用户的所有员工
    /// </summary>
    /// <param name="user"></param>
    /// <param name="type"></param>
    /// <param name="take"></param>
    /// <param name="cancellation"></param>
    /// <param name="skip"></param>
    /// <returns></returns>
    ValueTask<IEnumerable<StaffViewModel>> FindAllByUserAndPositionTypeAsync(IIdentifiable user, StaffPositionType type,
        int? skip = null,
        int? take = null, CancellationToken cancellation = default);

    /// <summary>
    ///     获取所有员工
    /// </summary>
    /// <param name="take"></param>
    /// <param name="cancellation"></param>
    /// <param name="skip"></param>
    /// <returns></returns>
    ValueTask<IEnumerable<StaffViewModel>> GetAllAsync(int? skip = null, int? take = null,
        CancellationToken cancellation = default);
}

/// <summary>
///     员工服务
/// </summary>
[Inject(ServiceLifetime.Singleton, typeof(IStaffRepository))]
internal class StaffRepository(
    IServiceProvider services,
    IUserRepository userRepository,
    DongKeJiDbContext coreDbContext
) : Repository<WorkDbContext, StaffEntity, StaffViewModel>(services), IStaffRepository
{
    public ValueTask<UserViewModel> FindUserByStaffAsync(IIdentifiable staff,
        CancellationToken cancellation = default)
    {
        return UnitOfWorkAsync(async _ =>
        {
            var staffEntity = await DbContext.Staffs
                .FirstOrDefaultAsync(x => x.Id == staff.Id, cancellation);

            staffEntity = staffEntity.IfThrowPrimaryKeyMissing(RepositoryActionType.Find, staff);

            if (staffEntity.UserId is null)
            {
                staffEntity.IfThrowForeignKeyMissing(RepositoryActionType.Find, staffEntity, nameof(staffEntity.UserId));
            }

            return await userRepository.FindByIdAsync(Identifiable.Create(staffEntity.UserId!.Value), cancellation);

        }, cancellation);
    }


    public ValueTask AddAsync(StaffViewModel staff, IIdentifiable user,
        CancellationToken cancellation = default)
    {
        return UnitOfWorkAsync(async _ =>
        {
            var staffEntity = await DbContext.Staffs
                .FirstOrDefaultAsync(x => x.Id == staff.Id, cancellation);

            staffEntity.IfThrowPrimaryKeyConflict(RepositoryActionType.Add, staff);

            var userEntity = await coreDbContext.Users
                .FirstOrDefaultAsync(x => x.Id == user.Id, cancellation);
            userEntity = userEntity.IfThrowPrimaryKeyMissing(RepositoryActionType.Add, user);

            //修改
            staffEntity = Mapper.Map<StaffEntity>(staff);
            staffEntity.UserId = userEntity.Id;

            //保存
            await DbContext.AddAsync(staffEntity, cancellation);
            await DbContext.IfThrowSaveFailedAsync(RepositoryActionType.Add, cancellation, staff, user);
            RegisterAutoUpdate(staff);

        }, cancellation);
    }


    public ValueTask<IEnumerable<StaffViewModel>> FindAllByUserAsync(IIdentifiable user,
        CancellationToken cancellation = default)
    {
        return UnitOfWorkAsync(async _ =>
        {
            var staffEntityList = await DbContext.Staffs
                .Where(x => x.UserId == user.Id)
                .ToListAsync(cancellation);

            return staffEntityList.Select(RegisterAutoUpdate);

        }, cancellation);
    }

    public ValueTask<IEnumerable<StaffViewModel>> FindAllByPositionTypeAsync(StaffPositionType type,
        int? skip = null, int? take = null, CancellationToken cancellation = default)
    {
        return UnitOfWorkAsync(async _ =>
        {
            var staffPositionEntity = await DbContext.StaffPositions
                .Include(x => x.Staffs)
                .Where(x => x.Type == type)
                .Select(x => x.Staffs
                    .OrderBy(staff => staff.Id)
                    .SkipAndTake(skip, take)
                    .ToList())
                .FirstOrDefaultAsync(cancellation) ?? [];

            return staffPositionEntity.Select(RegisterAutoUpdate);

        }, cancellation);
    }

    public ValueTask<IEnumerable<StaffViewModel>> FindAllByUserAndPositionTypeAsync(IIdentifiable user,
        StaffPositionType type, int? skip = null,
        int? take = null, CancellationToken cancellation = default)
    {
        return UnitOfWorkAsync(async _ =>
        {
            var positionEntity = await DbContext.StaffPositions
                .Include(x => x.Staffs)
                .Where(x => x.Type == type)
                .FirstOrDefaultAsync(cancellation);

            positionEntity = positionEntity.IfThrowPrimaryKeyMissing(RepositoryActionType.Find, user, type, skip, take);

            var staffEntityList = await DbContext.Staffs
                .Include(x => x.Positions)
                .Where(x => x.UserId == user.Id)
                .Where(x => !x.IsPrimaryAccount)
                .Where(x => x.Positions.Contains(positionEntity))
                .SkipAndTake(skip, take)
                .ToListAsync(cancellation);

            return staffEntityList.Select(RegisterAutoUpdate);

        }, cancellation);
    }


    public ValueTask<IEnumerable<StaffViewModel>> GetAllAsync(int? skip = null, int? take = null,
        CancellationToken cancellation = default)
    {
        return UnitOfWorkAsync(async _ =>
        {
            var staffEntityList = await DbContext.Staffs
                .SkipAndTake(skip, take)
                .ToListAsync(cancellation);

            return staffEntityList.Select(RegisterAutoUpdate);

        }, cancellation);
    }

}