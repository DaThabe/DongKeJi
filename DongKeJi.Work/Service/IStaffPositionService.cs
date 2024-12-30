using DongKeJi.Common;
using DongKeJi.Common.Exceptions;
using DongKeJi.Common.Extensions;
using DongKeJi.Common.Inject;
using DongKeJi.Common.Service;
using DongKeJi.Work.Model;
using DongKeJi.Work.Model.Entity.Staff;
using DongKeJi.Work.ViewModel.Common.Staff;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DongKeJi.Work.Service;

public interface IStaffPositionService
{
    /// <summary>
    ///     绑定职位
    /// </summary>
    ValueTask<StaffPositionViewModel> BindingAsync(
        StaffPositionType positionType, 
        IIdentifiable staff,
        CancellationToken cancellation = default);

    /// <summary>
    ///     解除绑定职位
    /// </summary>
    ValueTask UnbindingAsync(
        StaffPositionType positionType, 
        IIdentifiable staff,
        CancellationToken cancellation = default);

    /// <summary>
    ///     解除绑定员工的所有职位
    /// </summary>
    ValueTask UnbindingAsync(
        IIdentifiable staff, 
        CancellationToken cancellation = default);

    /// <summary>
    ///     设置职位信息, 有则更新, 没有则创建 (根据职位类型<see cref="StaffPositionType" />判断是否存在
    /// </summary>
    /// <param name="position"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    ValueTask SetAsync(
        StaffPositionViewModel position, 
        CancellationToken cancellation = default);

    /// <summary>
    ///     删除职位信息
    /// </summary>
    /// <param name="positionType"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    ValueTask RemoveAsync(
        StaffPositionType positionType, 
        CancellationToken cancellation = default);

    /// <summary>
    ///     根据类型查询职位信息
    /// </summary>
    /// <param name="positionType"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    ValueTask<StaffPositionViewModel> FindByTypeAsync(
        StaffPositionType positionType,
        CancellationToken cancellation = default);

    /// <summary>
    ///     查询员工的所有职位
    /// </summary>
    /// <param name="staff"></param>
    /// <param name="take"></param>
    /// <param name="cancellation"></param>
    /// <param name="skip"></param>
    /// <returns></returns>
    ValueTask<IEnumerable<StaffPositionViewModel>> FindAllByStaffAsync(
        IIdentifiable staff, 
        int? skip = null,
        int? take = null, 
        CancellationToken cancellation = default);

    /// <summary>
    ///     获取所有职位
    /// </summary>
    /// <param name="take"></param>
    /// <param name="cancellation"></param>
    /// <param name="skip"></param>
    /// <returns></returns>
    ValueTask<IEnumerable<StaffPositionViewModel>> GetAllAsync(
        int? skip = null, 
        int? take = null,
        CancellationToken cancellation = default);
}

[Inject(ServiceLifetime.Transient, typeof(IStaffPositionService))]
internal class StaffPositionService(IServiceProvider services) :
    Repository<WorkDbContext, StaffPositionEntity, StaffPositionViewModel>(services), IStaffPositionService
{
    public ValueTask<StaffPositionViewModel> BindingAsync(
        StaffPositionType positionType,
        IIdentifiable staff,
        CancellationToken cancellation = default)
    {
        return UnitOfWorkAsync(async _ =>
        {
            if (positionType == StaffPositionType.None)
            {
                throw new RepositoryException($"员工职位绑定失败, 职位不明确\n职位类型: {positionType}\n员工Id: {staff.Id}");
            }

            var staffEntity = await DbContext.Staffs
                .Include(x => x.Positions)
                .FirstOrDefaultAsync(x => x.Id == staff.Id, cancellation);

            if (staffEntity is null || staffEntity.IsEmpty())
            {
                throw new RepositoryException($"员工职位绑定失败, 员工不存在\n职位类型: {positionType}\n员工Id: {staff.Id}");
            }

            var staffPositionEntity = await DbContext.StaffPositions
                .Include(x => x.Staffs)
                .FirstOrDefaultAsync(x => x.Type == positionType, cancellation);

            if (staffPositionEntity is null || staffPositionEntity.IsEmpty())
            {
                staffPositionEntity = new StaffPositionEntity
                {
                    Type = positionType,
                    Title = positionType.ToString()
                };
            }

            //职位-员工 多对多
            staffPositionEntity.Staffs.Add(staffEntity, x => x.Id != staffEntity.Id);
            staffEntity.Positions.Add(staffPositionEntity, x => x.Type != staffPositionEntity.Type);

            //保存
            if (await DbContext.SaveChangesAsync(cancellation) <= 0)
            {
                throw new RepositoryException($"员工职位绑定失败, 未写入数据库\n职位类型: {positionType}\n员工Id: {staff.Id}");
            }

            return RegisterAutoUpdate(staffPositionEntity);

        }, cancellation);
    }
    public ValueTask UnbindingAsync(
        StaffPositionType positionType,
        IIdentifiable staff,
        CancellationToken cancellation = default)
    {
        return UnitOfWorkAsync(async _ =>
        {
            var staffEntity = await DbContext.Staffs
                .Include(x => x.Positions)
                .FirstOrDefaultAsync(x => x.Id == staff.Id, cancellation);

            if (staffEntity is null || staffEntity.IsEmpty())
            {
                throw new RepositoryException($"员工职位解除绑定失败, 员工不存在\n职位类型: {positionType}\n员工Id: {staff.Id}");
            }

            //保存
            staffEntity.Positions.Remove(x => x.Type == positionType);
            if (await DbContext.SaveChangesAsync(cancellation) <= 0)
            {
                throw new RepositoryException($"员工职位解除绑定失败, 未写入数据库\n职位类型: {positionType}\n员工Id: {staff.Id}");
            }

        }, cancellation);
    }

    public ValueTask UnbindingAsync(
        IIdentifiable staff,
        CancellationToken cancellation = default)
    {
        return UnitOfWorkAsync(async _ =>
        {
            var staffEntity = await DbContext.Staffs
                .Include(x => x.Positions)
                .FirstOrDefaultAsync(x => x.Id == staff.Id, cancellation);

            if (staffEntity is null || staffEntity.IsEmpty())
            {
                throw new RepositoryException($"员工职位清除失败, 员工不存在\n员工Id: {staff.Id}");
            }

            staffEntity.Positions.Clear();
            if (await DbContext.SaveChangesAsync(cancellation) <= 0)
            {
                throw new RepositoryException($"员工职位清除失败, 未写入数据库\n员工Id: {staff.Id}");
            }

        }, cancellation);
    }

    public ValueTask SetAsync(
        StaffPositionViewModel position,
        CancellationToken cancellation = default)
    {
        return UnitOfWorkAsync(async _ =>
        {
            if (position.Type == StaffPositionType.None)
            {
                throw new RepositoryException($"职位设置失败, 职位不明确\n职位信息: {position}");
            }

            var positionEntity = await DbContext.StaffPositions
                .FirstOrDefaultAsync(x => x.Type == position.Type, cancellation);

            if (positionEntity is null || positionEntity.IsEmpty())
            {
                positionEntity = Mapper.Map<StaffPositionEntity>(position);
                await DbContext.AddAsync(positionEntity, cancellation);
            }
            else
            {
                Mapper.Map(positionEntity, position);
            }

            if (await DbContext.SaveChangesAsync(cancellation) <= 0)
            {
                throw new RepositoryException($"职位设置失败, 未写入数据库\n职位信息: {position}");
            }

            RegisterAutoUpdate(position);

        }, cancellation);
    }

    public ValueTask RemoveAsync(
        StaffPositionType positionType,
        CancellationToken cancellation = default)
    {
        return UnitOfWorkAsync(async _ =>
        {
            var positionEntity = await DbContext.StaffPositions
                .FirstOrDefaultAsync(x => x.Type == positionType, cancellation);

            if (positionEntity is null || positionEntity.IsEmpty())
            {
                throw new RepositoryException($"职位删除失败, 数据不存在\n职位类型: {positionType}");
            }

            DbContext.Remove(positionEntity);
            if (await DbContext.SaveChangesAsync(cancellation) <= 0)
            {
                throw new RepositoryException($"职位删除失败, 未写入数据库\n职位类型: {positionType}");
            }

        }, cancellation);
    }

    public ValueTask<StaffPositionViewModel> FindByTypeAsync(
        StaffPositionType positionType,
        CancellationToken cancellation = default)
    {
        return UnitOfWorkAsync(async _ =>
        {
            var positionEntity = await DbContext.StaffPositions
                .FirstOrDefaultAsync(x => x.Type == positionType, cancellation);

            if (positionEntity is null || positionEntity.IsEmpty())
            {
                throw new RepositoryException($"查询职位失败, 数据不存在\n职位类型: {positionType}");
            }

            return RegisterAutoUpdate(positionEntity);

        }, cancellation);
    }

    public ValueTask<IEnumerable<StaffPositionViewModel>> FindAllByStaffAsync(
        IIdentifiable staff,
        int? skip = null,
        int? take = null,
        CancellationToken cancellation = default)
    {
        return UnitOfWorkAsync(async _ =>
        {
            var positionEntityList = await DbContext.Staffs
                .Include(x => x.Positions)
                .Where(x => x.Id == staff.Id)
                .Select(x => x.Positions.SkipAndTake(skip, take).ToList())
                .FirstOrDefaultAsync(cancellation);

            return positionEntityList?.Select(x => RegisterAutoUpdate(x)) ?? [];

        }, cancellation);
    }

    public ValueTask<IEnumerable<StaffPositionViewModel>> GetAllAsync(
        int? skip = null,
        int? take = null,
        CancellationToken cancellation = default)
    {
        return UnitOfWorkAsync(async _ =>
        {
            var positionEntityList = await DbContext.StaffPositions
                .SkipAndTake(skip, take)
                .ToListAsync(cancellation);

            return positionEntityList.Select(x => RegisterAutoUpdate(x));

        }, cancellation);
    }
}