using AutoMapper;
using DongKeJi.Common;
using DongKeJi.Common.Database;
using DongKeJi.Common.Extensions;
using DongKeJi.Common.Inject;
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
///     员工服务
/// </summary>
public interface IStaffService
{
    /// <summary>
    ///     根据员工查询用户
    /// </summary>
    /// <param name="staff"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    ValueTask<UserViewModel?> FindUserByStaffAsync(IIdentifiable staff, CancellationToken cancellation = default);

    /// <summary>
    ///     添加员工
    /// </summary>
    /// <param name="staff"></param>
    /// <param name="user"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    ValueTask<bool> AddAsync(StaffViewModel staff, IIdentifiable user, CancellationToken cancellation = default);

    /// <summary>
    ///     根据用户查询所有员工
    /// </summary>
    /// <param name="user"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    ValueTask<IEnumerable<StaffViewModel>> FindAllByUserAsync(IIdentifiable user, CancellationToken cancellation = default);

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
    ValueTask<IEnumerable<StaffViewModel>> FindAllByUserAndPositionTypeAsync(IIdentifiable user, StaffPositionType type, int? skip = null,
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
[Inject(ServiceLifetime.Singleton, typeof(IStaffService))]
internal class StaffService(
    IUserService userService,
    DongKeJiDbContext coreDbContext,
    PerformanceRecordDbContext dbContext,
    ILogger<StaffService> logger,
    IMapper mapper
) : IStaffService
{
    public async ValueTask<UserViewModel?> FindUserByStaffAsync(IIdentifiable staff,
        CancellationToken cancellation = default)
    {
        return await dbContext.UnitOfWorkAsync(async _ =>
        {
            var staffEntity = await dbContext.Staffs
                .FirstOrDefaultAsync(x => x.Id == staff.Id, cancellation);

            if (staffEntity?.UserId is null)
            {
                logger.LogWarning("查询员工绑定用户时发生错误, 无数据, 员工Id:{staff}", staff.Id);
                return null;
            }

            return await userService.FindByIdAsync(Identifiable.Create(staffEntity.UserId.Value), cancellation);
        }, ex => logger.LogError(ex, "查询员工绑定用户时发生错误, 用户Id:{staff}", staff.Id));
    }


    public async ValueTask<bool> AddAsync(StaffViewModel staff, IIdentifiable user,
        CancellationToken cancellation = default)
    {
        return await dbContext.UnitOfWorkAsync(async _ =>
        {
            var staffEntity = await dbContext.Staffs
                .FirstOrDefaultAsync(x => x.Id == staff.Id, cancellation);

            if (staffEntity is not null) throw new Exception("员工已存在");

            var userEntity = await coreDbContext.Users
                                 .FirstOrDefaultAsync(x => x.Id == user.Id, cancellation)
                             ?? throw new Exception("用户不存在");

            //修改
            staffEntity = mapper.Map<StaffEntity>(staff);
            staffEntity.UserId = userEntity.Id;

            //保存
            await dbContext.AddAsync(staffEntity, cancellation);
            RegisterAutoUpdate(staff);

            var result = await dbContext.SaveChangesAsync(cancellation);
            return result > 0;
        }, ex => logger.LogError(ex, "添加员工时发生错误, 员工Id:{staff}", staff.Id));
    }


    public async ValueTask<IEnumerable<StaffViewModel>> FindAllByUserAsync(IIdentifiable user,
        CancellationToken cancellation = default)
    {
        return await dbContext.UnitOfWorkAsync(async _ =>
        {
            var staffEntityList = await dbContext.Staffs
                .Where(x => x.UserId == user.Id)
                .ToListAsync(cancellation);

            if (staffEntityList is { Count: <=0})
            {
                logger.LogWarning("根据用户查询所有员工时发生错误, 无数据, 用户Id:{user}", user.Id);
                return [];
            }

            return staffEntityList.Select(RegisterAutoUpdate);

        }, ex => logger.LogError(ex, "根据用户查询所有员工时发生错误, 用户Id:{user}", user.Id)) ?? [];
    }

    public async ValueTask<IEnumerable<StaffViewModel>> FindAllByPositionTypeAsync(StaffPositionType type,
        int? skip = null, int? take = null, CancellationToken cancellation = default)
    {
        return await dbContext.UnitOfWorkAsync(async _ =>
        {
            var staffPositionEntity = await dbContext.StaffPositions
                .Include(x => x.Staffs)
                .Where(x => x.Type == type)
                .Select(x => x.Staffs
                    .OrderBy(staff => staff.Id)
                    .SkipAndTake(skip, take)
                    .ToList())
                .FirstOrDefaultAsync(cancellation);

            if (staffPositionEntity is null or { Count: < 0 })
            {
                logger.LogWarning("根据职位查询员工时发生错误, 指定职位不存在或没有员工, 职位类型:{type}", type);
                return [];
            }

            return staffPositionEntity.Select(RegisterAutoUpdate);
        }, ex => logger.LogError(ex, "根据职位查询员工时发生错误, 职位类型:{type}", type)) ?? [];
    }

    public async ValueTask<IEnumerable<StaffViewModel>> FindAllByUserAndPositionTypeAsync(IIdentifiable user, StaffPositionType type, int? skip = null,
        int? take = null, CancellationToken cancellation = default)
    {
        return await dbContext.UnitOfWorkAsync(async _ =>
        {
            var positionEntity = await dbContext.StaffPositions
                .Include(x => x.Staffs)
                .Where(x => x.Type == type)
                .FirstOrDefaultAsync(cancellationToken: cancellation);

            if (positionEntity is null) throw new Exception("没有符合条件的职位");

            var staffEntityList = await dbContext.Staffs
                .Include(x=>x.Positions)
                .Where(x=>x.UserId == user.Id)
                .Where(x=>!x.IsPrimaryAccount)
                .Where(x=>x.Positions.Contains(positionEntity))
                .SkipAndTake(skip, take)
                .ToListAsync(cancellation);

            if (staffEntityList is { Count: <= 0 }) throw new Exception("没有符合条件的员工");

            return staffEntityList.Select(RegisterAutoUpdate);

        }, ex => logger.LogError(ex, "根据用户和职位查询员工时发生错误, 职位Id:{staff}, 职位类型:{position}", user.Id, type)) ?? [];
    }


    public async ValueTask<IEnumerable<StaffViewModel>> GetAllAsync(int? skip = null, int? take = null,
        CancellationToken cancellation = default)
    {
        return await dbContext.UnitOfWorkAsync(async _ =>
        {
            var staffEntityList = await dbContext.Staffs
                .SkipAndTake(skip, take)
                .ToListAsync(cancellation);

            return staffEntityList.Select(RegisterAutoUpdate);
        }, ex => logger.LogError(ex, "获取所有员工时发生错误")) ?? [];
    }


    protected StaffViewModel RegisterAutoUpdate(StaffViewModel vm)
    {
        vm.PropertyChanged += async (_, _) =>
        {
            var existEntity = await dbContext.Staffs
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

    protected StaffViewModel RegisterAutoUpdate(StaffEntity staff)
    {
        var vm = mapper.Map<StaffViewModel>(staff);
        return RegisterAutoUpdate(vm);
    }
}