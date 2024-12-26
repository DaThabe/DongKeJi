using AutoMapper;
using DongKeJi.Common;
using DongKeJi.Common.Database;
using DongKeJi.Common.Extensions;
using DongKeJi.Common.Inject;
using DongKeJi.Work.Model;
using DongKeJi.Work.Model.Entity.Staff;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StaffPositionViewModel = DongKeJi.Work.ViewModel.Common.Staff.StaffPositionViewModel;

namespace DongKeJi.Work.Service;

public interface IStaffPositionService
{
    /// <summary>
    ///     绑定职位
    /// </summary>
    ValueTask<StaffPositionViewModel?> BindingAsync(StaffPositionType positionType, IIdentifiable staff,
        CancellationToken cancellation = default);

    /// <summary>
    ///     解除绑定职位
    /// </summary>
    ValueTask<bool> UnbindingAsync(StaffPositionType positionType, IIdentifiable staff,
        CancellationToken cancellation = default);

    /// <summary>
    ///     解除绑定员工的所有职位
    /// </summary>
    ValueTask<bool> UnbindingAsync(IIdentifiable staff, CancellationToken cancellation = default);


    /// <summary>
    ///     设置职位信息, 有则更新, 没有则创建 (根据职位类型<see cref="StaffPositionType" />判断是否存在
    /// </summary>
    /// <param name="position"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    ValueTask<bool> SetAsync(StaffPositionViewModel position, CancellationToken cancellation = default);

    /// <summary>
    ///     删除职位信息
    /// </summary>
    /// <param name="positionType"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    ValueTask<bool> RemoveAsync(StaffPositionType positionType, CancellationToken cancellation = default);

    /// <summary>
    ///     根据类型查询职位信息
    /// </summary>
    /// <param name="positionType"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    ValueTask<StaffPositionViewModel?> FindByTypeAsync(StaffPositionType positionType,
        CancellationToken cancellation = default);


    /// <summary>
    ///     查询员工的所有职位
    /// </summary>
    /// <param name="staff"></param>
    /// <param name="take"></param>
    /// <param name="cancellation"></param>
    /// <param name="skip"></param>
    /// <returns></returns>
    ValueTask<IEnumerable<StaffPositionViewModel>> FindAllByStaffAsync(IIdentifiable staff, int? skip = null,
        int? take = null, CancellationToken cancellation = default);

    /// <summary>
    ///     获取所有职位
    /// </summary>
    /// <param name="take"></param>
    /// <param name="cancellation"></param>
    /// <param name="skip"></param>
    /// <returns></returns>
    ValueTask<IEnumerable<StaffPositionViewModel>> GetAllAsync(int? skip = null, int? take = null,
        CancellationToken cancellation = default);
}

[Inject(ServiceLifetime.Transient, typeof(IStaffPositionService))]
internal class StaffPositionService(
    PerformanceRecordDbContext dbContext,
    ILogger<StaffPositionService> logger,
    IMapper mapper
) : IStaffPositionService
{
    public async ValueTask<StaffPositionViewModel?> BindingAsync(StaffPositionType positionType, IIdentifiable staff,
        CancellationToken cancellation = default)
    {
        return await dbContext.UnitOfWorkAsync(async _ =>
        {
            if (positionType == StaffPositionType.None)
            {
                logger.LogWarning("员工职位绑定失败, 职位不明确, 员工Id:{staff}, 职位:{position}", staff.Id, positionType);
                return null;
            }

            var staffEntity = await dbContext.Staffs
                .Include(x => x.Positions)
                .FirstOrDefaultAsync(x => x.Id == staff.Id, cancellation);

            if (staffEntity is null)
            {
                logger.LogWarning("员工职位绑定失败, 员工不存在, 员工Id:{staff}, 职位:{position}", staff.Id, positionType);
                return null;
            }

            var staffPositionEntity = await dbContext.StaffPositions
                                          .Include(x => x.Staffs)
                                          .FirstOrDefaultAsync(x => x.Type == positionType, cancellation)
                                      ?? new StaffPositionEntity
                                          { Type = positionType, Title = positionType.ToString() };

            //职位-员工 多对多
            staffPositionEntity.Staffs.Add(staffEntity, x => x.Id != staffEntity.Id);
            staffEntity.Positions.Add(staffPositionEntity, x => x.Type != staffPositionEntity.Type);

            //保存
            await dbContext.SaveChangesAsync(cancellation);
            return RegisterAutoUpdate(staffPositionEntity);
        }, ex => logger.LogError(ex, "员工职位绑定失败, 员工Id:{staff}, 职位:{position}", staff.Id, positionType));
    }

    public async ValueTask<bool> UnbindingAsync(StaffPositionType positionType, IIdentifiable staff,
        CancellationToken cancellation = default)
    {
        return await dbContext.UnitOfWorkAsync(async _ =>
        {
            var staffEntity = await dbContext.Staffs
                .Include(x => x.Positions)
                .FirstOrDefaultAsync(x => x.Id == staff.Id, cancellation);

            if (staffEntity is null)
            {
                logger.LogWarning("员工职位解除绑定失败, 员工不存在, 员工Id:{staff}", staff.Id);
                return false;
            }

            //保存
            staffEntity.Positions.Remove(x => x.Type == positionType);
            var result = await dbContext.SaveChangesAsync(cancellation);
            return result > 0;
        }, ex => logger.LogError(ex, "员工职位解除绑定失败, 员工Id:{staff}, 职位:{position}", staff.Id, positionType));
    }

    public async ValueTask<bool> UnbindingAsync(IIdentifiable staff, CancellationToken cancellation = default)
    {
        return await dbContext.UnitOfWorkAsync(async _ =>
        {
            var staffEntity = await dbContext.Staffs
                .Include(x => x.Positions)
                .FirstOrDefaultAsync(x => x.Id == staff.Id, cancellation);

            if (staffEntity is null)
            {
                logger.LogWarning("员工职位解除绑定失败, 员工不存在, 员工Id:{staff}", staff.Id);
                return false;
            }

            staffEntity.Positions.Clear();
            var result = await dbContext.SaveChangesAsync(cancellation);
            return result > 0;
        }, ex => logger.LogError(ex, "员工职位解除绑定失败, 员工Id:{staff}", staff.Id));
    }


    public async ValueTask<bool> SetAsync(StaffPositionViewModel position, CancellationToken cancellation = default)
    {
        return await dbContext.UnitOfWorkAsync(async _ =>
        {
            if (position.Type is not (StaffPositionType.Designer or StaffPositionType.Salesperson))
                throw new Exception($"职位不明确: {position.Type}");

            var positionEntity = await dbContext.StaffPositions
                .FirstOrDefaultAsync(x => x.Type == position.Type, cancellation);

            if (positionEntity is null)
            {
                positionEntity = mapper.Map<StaffPositionEntity>(position);
                await dbContext.AddAsync(positionEntity, cancellation);
                await dbContext.SaveChangesAsync(cancellation);
            }
            else
            {
                mapper.Map(positionEntity, position);
            }

            RegisterAutoUpdate(position);
            return true;
        }, ex => logger.LogError(ex, "设置职位时发生错误, 类型:{type},标题:{title}", position.Type, position.Title));
    }

    public async ValueTask<bool> RemoveAsync(StaffPositionType positionType, CancellationToken cancellation = default)
    {
        return await dbContext.UnitOfWorkAsync(async _ =>
        {
            var positionEntity = await dbContext.StaffPositions
                .FirstOrDefaultAsync(x => x.Type == positionType, cancellation);

            if (positionEntity is null)
            {
                logger.LogWarning("没有合适的职位可以删除, 类型:{type}", positionType);
                return false;
            }

            dbContext.Remove(positionEntity);
            var result = await dbContext.SaveChangesAsync(cancellation);
            return result > 0;
        }, ex => logger.LogError(ex, "删除职位时发生错误, 类型:{type}", positionType));
    }


    public async ValueTask<StaffPositionViewModel?> FindByTypeAsync(StaffPositionType positionType,
        CancellationToken cancellation = default)
    {
        return await dbContext.UnitOfWorkAsync(async _ =>
        {
            var positionEntity = await dbContext.StaffPositions
                .FirstOrDefaultAsync(x => x.Type == positionType, cancellation);

            if (positionEntity is null)
            {
                logger.LogWarning("查询职位失败, 职位不存在, 类型:{type}", positionType);
                return null;
            }

            return RegisterAutoUpdate(positionEntity);
        }, ex => logger.LogError(ex, "查询职位失败, 职位不存在, 类型:{type}", positionType));
    }

    public async ValueTask<IEnumerable<StaffPositionViewModel>> FindAllByStaffAsync(IIdentifiable staff,
        int? skip = null, int? take = null, CancellationToken cancellation = default)
    {
        return await dbContext.UnitOfWorkAsync(async _ =>
        {
            var positionEntityList = await dbContext.Staffs
                .Include(x => x.Positions)
                .Where(x => x.Id == staff.Id)
                .Select(x => x.Positions.SkipAndTake(skip, take).ToList())
                .FirstOrDefaultAsync(cancellation);

            if (positionEntityList is null || positionEntityList.Count <= 0)
            {
                logger.LogWarning("查询员工所有职位查询失败, 员工不存在, 或没有关联职位, 员工Id:{staff}", staff.Id);
                return [];
            }

            return positionEntityList.Select(RegisterAutoUpdate);
        }, ex => logger.LogError(ex, "查询员工所有职位查询失败, 员工Id:{staff}", staff.Id)) ?? [];
    }

    public async ValueTask<IEnumerable<StaffPositionViewModel>> GetAllAsync(int? skip = null, int? take = null,
        CancellationToken cancellation = default)
    {
        return await dbContext.UnitOfWorkAsync(async _ =>
        {
            var positionEntityList = await dbContext.StaffPositions
                .SkipAndTake(skip, take)
                .ToListAsync(cancellation);

            if (positionEntityList.Count <= 0)
            {
                logger.LogWarning("获取所有职位查询失败, 没有任何职位信息");
                return [];
            }

            return positionEntityList.Select(RegisterAutoUpdate);
        }, ex => logger.LogError(ex, "获取所有职位查询失败, 没有任何职位信息")) ?? [];
    }


    protected StaffPositionViewModel RegisterAutoUpdate(StaffPositionViewModel vm)
    {
        vm.PropertyChanged += async (_, _) =>
        {
            var existEntity = await dbContext.StaffPositions
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

    protected StaffPositionViewModel RegisterAutoUpdate(StaffPositionEntity staff)
    {
        var vm = mapper.Map<StaffPositionViewModel>(staff);
        return RegisterAutoUpdate(vm);
    }
}