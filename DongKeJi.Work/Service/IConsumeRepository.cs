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
    ValueTask<IEnumerable<TimingConsumeViewModel>> GetAllByTimingOrderAsync(
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
    ValueTask<IEnumerable<CountingConsumeViewModel>> GetAllByCountingOrderAsync(
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
    ValueTask<IEnumerable<MixingConsumeViewModel>> GetAllByMixingOrderAsync(
        IIdentifiable order,
        int? skip = null,
        int? take = null,
        CancellationToken cancellation = default);
}

/// <summary>
///     划扣服务
/// </summary>
[Inject(ServiceLifetime.Singleton, typeof(IConsumeRepository))]
internal class ConsumeRepository(IServiceProvider services
) : Repository<WorkDbContext, ConsumeEntity, ConsumeViewModel>(services), IConsumeRepository
{
    public ValueTask AddAsync(
        ConsumeViewModel consume,
        IIdentifiable order,
        IIdentifiable staff,
        CancellationToken cancellation = default)
    {
        return UnitOfWorkAsync(async _ =>
        {
            var consumeEntity = await DbContext.Consumes
                .FirstOrDefaultAsync(x => x.Id == consume.Id, cancellation);

            if (consumeEntity != null && !consumeEntity.IsEmpty())
            {
                throw new RepositoryException($"划扣添加失败, 相同Id已存在\n划扣信息: {staff}");
            }

            var staffEntity = await DbContext.Staffs
                .Include(x => x.Customers)
                .Include(x => x.Orders)
                .FirstOrDefaultAsync(x => x.Id == staff.Id, cancellation);

            if (staffEntity == null || staffEntity.IsEmpty())
            {
                throw new RepositoryException($"划扣添加失败, 关联员工不存在\n员工Id: {staff.Id}");
            }

            //创建实体
            consumeEntity = Mapper.Map<ConsumeEntity>(consume);
            await Handel(staffEntity, consumeEntity);

            //保存
            await DbContext.AddAsync(consumeEntity, cancellation);
            if (await DbContext.SaveChangesAsync(cancellation) <= 0)
            {
                throw new RepositoryException($"划扣添加失败, 未写入数据库\n划扣信息: {consume}\n订单Id: {order.Id}\n员工Id: {staff.Id}");
            }
            RegisterAutoUpdate(consume);

        }, cancellation);


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
            var orderEntity = await DbContext.TimingOrders
                .Include(x => x.Staffs)
                .Include(x => x.Consumes)
                .Where(x => x.Id == order.Id)
                .FirstOrDefaultAsync(cancellation);

            if (orderEntity is null || orderEntity.IsEmpty())
            {
                throw new RepositoryException($"划扣添加失败, 关联订单不存在, 订单Id: {order.Id}");
            }

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
            var orderEntity = await DbContext.CountingOrders
                .Include(x => x.Staffs)
                .Include(x => x.Consumes)
                .Where(x => x.Id == order.Id)
                .FirstOrDefaultAsync(cancellation);

            if (orderEntity is null || orderEntity.IsEmpty())
            {
                throw new RepositoryException($"划扣添加失败, 关联订单不存在, 订单Id: {order.Id}");
            }

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
            var orderEntity = await DbContext.MixingOrders
                .Include(x => x.Staffs)
                .Include(x => x.Consumes)
                .Where(x => x.Id == order.Id)
                .FirstOrDefaultAsync(cancellation);

            if (orderEntity is null || orderEntity.IsEmpty())
            {
                throw new RepositoryException($"划扣添加失败, 关联订单不存在, 订单Id: {order.Id}");
            }

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

    public ValueTask RemoveAsync(
        IIdentifiable consume,
        CancellationToken cancellation = default)
    {
        return UnitOfWorkAsync(async _ =>
        {
            var consumeEntity = await DbContext.Consumes
            .FirstOrDefaultAsync(x => x.Id == consume.Id, cancellation);

            if (consumeEntity is null || consumeEntity.IsEmpty())
            {
                throw new RepositoryException($"划扣删除失败, 数据不存在, 划扣Id: {consume.Id}");
            }

            DbContext.Remove(consumeEntity);
            if (await DbContext.SaveChangesAsync(cancellation) <= 0)
            {
                throw new RepositoryException($"划扣删除失败, 未写入数据库\n划扣Id: {consume.Id}");
            }

        }, cancellation);
    }

    public async ValueTask<IEnumerable<TimingConsumeViewModel>> GetAllByTimingOrderAsync(
        IIdentifiable order,
        int? skip = null,
        int? take = null,
        CancellationToken cancellation = default)
    {
        return await UnitOfWorkAsync(async _ =>
        {
            var entityList = await DbContext.TimingConsumes
                .OrderBy(x => x.Id)
                .Where(x => x.OrderId == order.Id)
                .SkipAndTake(skip, take)
                .ToListAsync(cancellation);

            return entityList.Select(RegisterAutoUpdate<TimingConsumeViewModel>);

        }, cancellation);
    }

    public ValueTask<IEnumerable<CountingConsumeViewModel>> GetAllByCountingOrderAsync(
        IIdentifiable order,
        int? skip = null,
        int? take = null,
        CancellationToken cancellation = default)
    {
        return UnitOfWorkAsync(async _ =>
        {
            var entityList = await DbContext.CountingConsumes
                .OrderBy(x => x.Id)
                .Where(x => x.OrderId == order.Id)
                .SkipAndTake(skip, take)
                .ToListAsync(cancellation);

            return entityList.Select(RegisterAutoUpdate<CountingConsumeViewModel>);

        }, cancellation);
    }

    public ValueTask<IEnumerable<MixingConsumeViewModel>> GetAllByMixingOrderAsync(
        IIdentifiable order,
        int? skip = null,
        int? take = null,
        CancellationToken cancellation = default)
    {
        return UnitOfWorkAsync(async _ =>
        {
            var entityList = await DbContext.MixingConsumes
                .OrderBy(x => x.Id)
                .Where(x => x.OrderId == order.Id)
                .SkipAndTake(skip, take)
                .ToListAsync(cancellation);

            return entityList.Select(RegisterAutoUpdate<MixingConsumeViewModel>);

        }, cancellation);
    }
}