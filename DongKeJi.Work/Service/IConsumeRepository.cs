using AutoMapper;
using DongKeJi.Common;
using DongKeJi.Common.Exceptions;
using DongKeJi.Common.Extensions;
using DongKeJi.Common.Inject;
using DongKeJi.Common.Service;
using DongKeJi.Work.Model;
using DongKeJi.Work.Model.Entity.Consume;
using DongKeJi.Work.Model.Entity.Staff;
using DongKeJi.Work.ViewModel.Common.Consume;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DongKeJi.Work.Service;

/// <summary>
///     划扣服务
/// </summary>
public interface IConsumeRepository
{
    /// <summary>
    ///     创建划扣且关联用户和订单
    /// </summary>
    /// <param name="consume"></param>
    /// <param name="order"></param>
    /// <param name="staff"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    ValueTask AddAsync(
        ConsumeViewModel consume,
        IIdentifiable order,
        IIdentifiable staff,
        CancellationToken cancellation = default);

    /// <summary>
    ///     删除划扣
    /// </summary>
    /// <param name="consume"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    ValueTask RemoveAsync(
        IIdentifiable consume,
        CancellationToken cancellation = default);


    /// <summary>
    ///     获取所有计时订单关联的划扣
    /// </summary>
    /// <param name="order"></param>
    /// <param name="skip"></param>
    /// <param name="take"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    ValueTask<IEnumerable<ConsumeTimingViewModel>> GetAllByTimingOrderAsync(
        IIdentifiable order,
        int? skip = null,
        int? take = null,
        CancellationToken cancellation = default);

    /// <summary>
    ///     获取所有计数订单关联的划扣
    /// </summary>
    /// <param name="order"></param>
    /// <param name="skip"></param>
    /// <param name="take"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    ValueTask<IEnumerable<ConsumeCountingViewModel>> GetAllByCountingOrderAsync(
        IIdentifiable order,
        int? skip = null,
        int? take = null,
        CancellationToken cancellation = default);

    /// <summary>
    ///     获取所有混合订单关联的划扣
    /// </summary>
    /// <param name="order"></param>
    /// <param name="skip"></param>
    /// <param name="take"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    ValueTask<IEnumerable<ConsumeMixingViewModel>> GetAllByMixingOrderAsync(
        IIdentifiable order,
        int? skip = null,
        int? take = null,
        CancellationToken cancellation = default);
}

/// <summary>
///     划扣服务
/// </summary>
[Inject(ServiceLifetime.Singleton, typeof(IConsumeRepository))]
internal class ConsumeRepository(
    IServiceProvider services,
    IMapper mapper,
    WorkDbContext dbContext) : 
    IConsumeRepository
{
    public async ValueTask AddAsync(
        ConsumeViewModel consume,
        IIdentifiable order,
        IIdentifiable staff,
        CancellationToken cancellation = default)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellation);

        try
        {
            //验证
            consume.Validate();

            //划扣
            var consumeEntity = await dbContext.Consumes
                .FirstOrDefaultAsync(x => x.Id == consume.Id, cancellation);
            RepositoryException.ThrowIfEntityAlreadyExists(consumeEntity, "划扣已存在");

            //员工
            var staffEntity = await dbContext.Staffs
                .Include(x => x.Customers)
                .Include(x => x.Orders)
                .FirstOrDefaultAsync(x => x.Id == staff.Id, cancellation);
            staffEntity = RepositoryException.ThrowIfEntityNotFound(staffEntity, "关联员工不存在");


            //创建实体
            consumeEntity = mapper.Map<ConsumeEntity>(consume);
            await Handel(staffEntity, consumeEntity);

            //保存
            await dbContext.AddAsync(consumeEntity, cancellation);

            await dbContext.AssertSaveSuccessAsync(cancellation: cancellation);
            await transaction.CommitAsync(cancellation);

            dbContext.RegisterAutoUpdate<ConsumeEntity, ConsumeViewModel>(consume, services);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellation);
            throw new RepositoryException($"添加划扣时发生错误\n划扣信息: {consume}\n订单Id: {order.Id}\n员工Id: {staff.Id}", ex);
        }

        return;

        async ValueTask Handel(StaffEntity staffEntity, ConsumeEntity consumeEntity)
        {
            switch (consumeEntity)
            {
                case TimingConsumeEntity t:
                    await Timing(staffEntity, t);
                    return;
                case CountingConsumeEntity c:
                    await Counting(staffEntity, c);
                    return;
                case MixingConsumeEntity m:
                    await Mixing(staffEntity, m);
                    return;
                default:
                    throw new RepositoryException($"添加划扣失败, 数据无法解析, 类型:{consumeEntity.GetType().Name}");
            }
        }

        async ValueTask Timing(StaffEntity staffEntity, TimingConsumeEntity timingConsumeEntity)
        {
            var orderEntity = await dbContext.TimingOrders
                .Include(x => x.Staffs)
                .Include(x => x.Consumes)
                .Where(x => x.Id == order.Id)
                .FirstOrDefaultAsync(cancellation);
            orderEntity = RepositoryException.ThrowIfEntityNotFound(orderEntity, "关联订单不存在");

            //员工-划扣 多对多
            staffEntity.Consumes.Add(timingConsumeEntity);
            timingConsumeEntity.Staff = staffEntity;

            //员工-订单 多对多
            staffEntity.Orders.Add(orderEntity, x => x.Id != orderEntity.Id);
            orderEntity.Staffs.Add(staffEntity, x => x.Id != staffEntity.Id);

            //订单-划扣 一对多
            orderEntity.Consumes.Add(timingConsumeEntity);
            timingConsumeEntity.Order = orderEntity;
        }

        async ValueTask Counting(StaffEntity staffEntity, CountingConsumeEntity countingConsumeEntity)
        {
            var orderEntity = await dbContext.CountingOrders
                .Include(x => x.Staffs)
                .Include(x => x.Consumes)
                .Where(x => x.Id == order.Id)
                .FirstOrDefaultAsync(cancellation);
            orderEntity = RepositoryException.ThrowIfEntityNotFound(orderEntity, "关联订单不存在");

            //员工-划扣 多对多
            staffEntity.Consumes.Add(countingConsumeEntity);
            countingConsumeEntity.Staff = staffEntity;

            //员工-订单 多对多
            staffEntity.Orders.Add(orderEntity, x => x.Id != orderEntity.Id);
            orderEntity.Staffs.Add(staffEntity, x => x.Id != staffEntity.Id);

            //订单-划扣 一对多
            orderEntity.Consumes.Add(countingConsumeEntity);
            countingConsumeEntity.Order = orderEntity;
        }

        async ValueTask Mixing(StaffEntity staffEntity, MixingConsumeEntity mixingConsumeEntity)
        {
            var orderEntity = await dbContext.MixingOrders
                .Include(x => x.Staffs)
                .Include(x => x.Consumes)
                .Where(x => x.Id == order.Id)
                .FirstOrDefaultAsync(cancellation);
            orderEntity = RepositoryException.ThrowIfEntityNotFound(orderEntity, "关联订单不存在");

            //员工-划扣 多对多
            staffEntity.Consumes.Add(mixingConsumeEntity);
            mixingConsumeEntity.Staff = staffEntity;

            //员工-订单 多对多
            staffEntity.Orders.Add(orderEntity, x => x.Id != orderEntity.Id);
            orderEntity.Staffs.Add(staffEntity, x => x.Id != staffEntity.Id);

            //订单-划扣 一对多
            orderEntity.Consumes.Add(mixingConsumeEntity);
            mixingConsumeEntity.Order = orderEntity;
        }
    }

    public async ValueTask RemoveAsync(
        IIdentifiable consume,
        CancellationToken cancellation = default)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellation);

        try
        {
            var consumeEntity = await dbContext.Consumes
                .FirstOrDefaultAsync(x => x.Id == consume.Id, cancellation);
            consumeEntity = RepositoryException.ThrowIfEntityNotFound(consumeEntity, "划扣不存在");

            dbContext.Remove(consumeEntity);

            await dbContext.AssertSaveSuccessAsync(cancellation: cancellation);
            await transaction.CommitAsync(cancellation);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellation);
            throw new RepositoryException($"删除划扣时发生错误\n划扣Id: {consume.Id}", ex);
        }
    }

    public async ValueTask<IEnumerable<ConsumeTimingViewModel>> GetAllByTimingOrderAsync(
        IIdentifiable order,
        int? skip = null,
        int? take = null,
        CancellationToken cancellation = default)
    {
        //await using var transaction = await DbContext.Database.BeginTransactionAsync(cancellation);

        try
        {
            var entityList = await dbContext.TimingConsumes
                .OrderBy(x => x.Id)
                .Where(x => x.OrderId == order.Id)
                .SkipAndTake(skip, take)
                .ToListAsync(cancellation);

            return entityList.Select(x =>
                dbContext.RegisterAutoUpdate<TimingConsumeEntity, ConsumeTimingViewModel>(x, services));
        }
        catch (Exception ex)
        {
            //await transaction.RollbackAsync(cancellation);
            throw new RepositoryException($"获取计时订单下所有划扣时发生错误\n订单Id: {order.Id}", ex);
        }
    }

    public async ValueTask<IEnumerable<ConsumeCountingViewModel>> GetAllByCountingOrderAsync(
        IIdentifiable order,
        int? skip = null,
        int? take = null,
        CancellationToken cancellation = default)
    {
        //await using var transaction = await DbContext.Database.BeginTransactionAsync(cancellation);

        try
        {
            var entityList = await dbContext.CountingConsumes
                .OrderBy(x => x.Id)
                .Where(x => x.OrderId == order.Id)
                .SkipAndTake(skip, take)
                .ToListAsync(cancellation);

            return entityList.Select(x =>
                dbContext.RegisterAutoUpdate<CountingConsumeEntity, ConsumeCountingViewModel>(x, services));
        }
        catch (Exception ex)
        {
            //await transaction.RollbackAsync(cancellation);
            throw new RepositoryException($"获取计时订单下所有划扣时发生错误\n订单Id: {order.Id}", ex);
        }
    }

    public async ValueTask<IEnumerable<ConsumeMixingViewModel>> GetAllByMixingOrderAsync(
        IIdentifiable order,
        int? skip = null,
        int? take = null,
        CancellationToken cancellation = default)
    {
        //await using var transaction = await DbContext.Database.BeginTransactionAsync(cancellation);

        try
        {
            var entityList = await dbContext.MixingConsumes
                .OrderBy(x => x.Id)
                .Where(x => x.OrderId == order.Id)
                .SkipAndTake(skip, take)
                .ToListAsync(cancellation);

            return entityList.Select(x =>
                dbContext.RegisterAutoUpdate<MixingConsumeEntity, ConsumeMixingViewModel>(x, services));
        }
        catch (Exception ex)
        {
            //await transaction.RollbackAsync(cancellation);
            throw new RepositoryException($"获取混合订单下所有划扣时发生错误\n订单Id: {order.Id}", ex);
        }
    }
}