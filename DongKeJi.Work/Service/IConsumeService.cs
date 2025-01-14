using AutoMapper;
using DongKeJi.Database;
using DongKeJi.Exceptions;
using DongKeJi.Extensions;
using DongKeJi.Inject;
using DongKeJi.Validation;
using DongKeJi.Work.Model;
using DongKeJi.Work.Model.Entity.Consume;
using DongKeJi.Work.Model.Entity.Order;
using DongKeJi.Work.Model.Entity.Staff;
using DongKeJi.Work.ViewModel.Consume;
using DongKeJi.Work.ViewModel.Order;
using DongKeJi.Work.ViewModel.Staff;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DongKeJi.Work.Service;

/// <summary>
///     划扣服务
/// </summary>
public interface IConsumeService
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
    /// 获取划扣的设计师
    /// </summary>
    /// <param name="consume"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    ValueTask<StaffViewModel> FindDesignerAsync(
        IIdentifiable consume,
        CancellationToken cancellation = default);

    /// <summary>
    /// 设置划扣的设计师
    /// </summary>
    /// <param name="consume"></param>
    /// <param name="designer"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    ValueTask SetDesignerAsync(
        IIdentifiable consume,
        IIdentifiable designer,
        CancellationToken cancellation = default);

    /// <summary>
    /// 获取指定年月的所有划扣
    /// </summary>
    /// <typeparam name="TConsume">划扣类型-也对应订单类型</typeparam>
    /// <param name="order"></param>
    /// <param name="yearMonthly"></param>
    /// <param name="skip"></param>
    /// <param name="take"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    ValueTask<IEnumerable<TConsume>> FindAllByYearMonthlyAsync<TConsume>(
        IIdentifiable order,
        DateTime yearMonthly,
        int? skip = null,
        int? take = null,
        CancellationToken cancellation = default) where TConsume : ConsumeViewModel;

    /// <summary>
    /// 查询所有订单关联的划扣
    /// </summary>
    /// <typeparam name="TConsume">划扣类型-也对应订单类型</typeparam>
    /// <param name="order"></param>
    /// <param name="skip"></param>
    /// <param name="take"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    ValueTask<IEnumerable<TConsume>> FindAllByOrderAsync<TConsume>(
        IIdentifiable order,
        int? skip = null,
        int? take = null,
        CancellationToken cancellation = default) where TConsume : ConsumeViewModel;
}


public static class ConsumeServiceExtensions
{
    public static async ValueTask<IEnumerable<ConsumeViewModel>> FindAllConsumeAsync(this IConsumeService consumeService, OrderViewModel order)
    {
        return order.Type switch
        {
            OrderType.Timing => await consumeService.FindAllByOrderAsync<ConsumeTimingViewModel>(order),
            OrderType.Counting => await consumeService.FindAllByOrderAsync<ConsumeCountingViewModel>(order),
            OrderType.Mixing => await consumeService.FindAllByOrderAsync<ConsumeMixingViewModel>(order),
            _ => []
        };
    }

    public static async ValueTask<IEnumerable<ConsumeViewModel>> FindAllConsumeAsync(this IConsumeService consumeService, OrderViewModel order, DateTime date)
    {
        return order.Type switch
        {
            OrderType.Timing => await consumeService.FindAllByYearMonthlyAsync<ConsumeTimingViewModel>(order, date),
            OrderType.Counting => await consumeService.FindAllByYearMonthlyAsync<ConsumeCountingViewModel>(order, date),
            OrderType.Mixing => await consumeService.FindAllByYearMonthlyAsync<ConsumeMixingViewModel>(order, date),
            _ => []
        };
    }

    public static ConsumeViewModel? CreateDefaultConsume(this OrderType order, DateTime date)
    {
        return order switch
        {
            OrderType.Timing => new ConsumeTimingViewModel { ConsumeDays = 1, CreateTime = date },
            OrderType.Counting => new ConsumeCountingViewModel { ConsumeCounts = 1, CreateTime = date },
            OrderType.Mixing => new ConsumeMixingViewModel { ConsumeDays = 1, ConsumeCounts = 0, CreateTime = date },
            _ => null
        };
    }
}


/// <summary>
///     划扣服务
/// </summary>
[Inject(ServiceLifetime.Singleton, typeof(IConsumeService))]
internal class ConsumeService(WorkDbContext dbContext, IMapper mapper) : IConsumeService
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
            consume.AssertValidate();

            //划扣
            var consumeEntity = await dbContext.Consume
                .FirstOrDefaultAsync(x => x.Id == consume.Id, cancellation);
            DatabaseException.ThrowIfEntityAlreadyExists(consumeEntity, "划扣已存在");

            //员工
            var staffEntity = await dbContext.Staff
                .Include(x => x.Customers)
                .Include(x => x.Orders)
                .FirstOrDefaultAsync(x => x.Id == staff.Id, cancellation);
            staffEntity = DatabaseException.ThrowIfEntityNotFound(staffEntity, "关联员工不存在");


            //创建实体
            consumeEntity = mapper.Map<ConsumeEntity>(consume);
            await Handel(staffEntity, consumeEntity);

            //保存
            await dbContext.AddAsync(consumeEntity, cancellation);

            await dbContext.AssertSaveChangesAsync(cancellation: cancellation);
            await transaction.CommitAsync(cancellation);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellation);
            throw new DatabaseException($"添加划扣时发生错误\n划扣信息: {consume}\n订单Id: {order.Id}\n员工Id: {staff.Id}", ex);
        }

        return;

        async ValueTask Handel(StaffEntity staffEntity, ConsumeEntity consumeEntity)
        {
            switch (consumeEntity)
            {
                case ConsumeTimingEntity t:
                    await Timing(staffEntity, t);
                    return;
                case ConsumeCountingEntity c:
                    await Counting(staffEntity, c);
                    return;
                case ConsumeMixingEntity m:
                    await Mixing(staffEntity, m);
                    return;
                default:
                    throw new DatabaseException($"添加划扣失败, 数据无法解析, 类型:{consumeEntity.GetType().Name}");
            }
        }

        async ValueTask Timing(StaffEntity staffEntity, ConsumeTimingEntity timingConsumeEntity)
        {
            var orderEntity = await dbContext.OrderTiming
                .Include(x => x.Staffs)
                .Include(x => x.Consumes)
                .Where(x => x.Id == order.Id)
                .FirstOrDefaultAsync(cancellation);
            orderEntity = DatabaseException.ThrowIfEntityNotFound(orderEntity, "关联订单不存在");

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

        async ValueTask Counting(StaffEntity staffEntity, ConsumeCountingEntity countingConsumeEntity)
        {
            var orderEntity = await dbContext.OrderCounting
                .Include(x => x.Staffs)
                .Include(x => x.Consumes)
                .Where(x => x.Id == order.Id)
                .FirstOrDefaultAsync(cancellation);
            orderEntity = DatabaseException.ThrowIfEntityNotFound(orderEntity, "关联订单不存在");

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

        async ValueTask Mixing(StaffEntity staffEntity, ConsumeMixingEntity mixingConsumeEntity)
        {
            var orderEntity = await dbContext.OrderMixing
                .Include(x => x.Staffs)
                .Include(x => x.Consumes)
                .Where(x => x.Id == order.Id)
                .FirstOrDefaultAsync(cancellation);
            orderEntity = DatabaseException.ThrowIfEntityNotFound(orderEntity, "关联订单不存在");

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
            var consumeEntity = await dbContext.Consume
                .FirstOrDefaultAsync(x => x.Id == consume.Id, cancellation);
            consumeEntity = DatabaseException.ThrowIfEntityNotFound(consumeEntity, "划扣不存在");

            dbContext.Remove(consumeEntity);

            await dbContext.AssertSaveChangesAsync(cancellation: cancellation);
            await transaction.CommitAsync(cancellation);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellation);
            throw new DatabaseException($"删除划扣时发生错误\n划扣Id: {consume.Id}", ex);
        }
    }

    public async ValueTask<StaffViewModel> FindDesignerAsync(
        IIdentifiable consume, 
        CancellationToken cancellation = default)
    {
        //await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellation);

        try
        {
            var consumeEntity = await dbContext.Consume
                .Include(x=>x.Staff)
                .FirstOrDefaultAsync(x => x.Id == consume.Id, cancellation);
            consumeEntity = DatabaseException.ThrowIfEntityNotFound(consumeEntity, "划扣不存在");

            return mapper.Map<StaffViewModel>(consumeEntity.Staff);
        }
        catch (Exception ex)
        {
            //await transaction.RollbackAsync(cancellation);
            throw new DatabaseException($"查询划扣设计师时发生错误\n划扣Id: {consume.Id}", ex);
        }
    }

    public async ValueTask SetDesignerAsync(
        IIdentifiable consume, 
        IIdentifiable designer, 
        CancellationToken cancellation = default)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellation);

        try
        {
            var staffEntity = await dbContext.Staff
                .FirstOrDefaultAsync(x => x.Id == designer.Id, cancellation);
            staffEntity = DatabaseException.ThrowIfEntityNotFound(staffEntity, "设计师不存在");

 
            var consumeEntity = await dbContext.Consume
                .Include(x => x.Staff)
                .FirstOrDefaultAsync(x => x.Id == consume.Id, cancellation);
            consumeEntity = DatabaseException.ThrowIfEntityNotFound(consumeEntity, "划扣不存在");


            consumeEntity.Staff = staffEntity;
            staffEntity.Consumes.Add(consumeEntity, x => x.Id != consumeEntity.Id);

            await dbContext.AssertSaveChangesAsync(cancellation: cancellation);
            await transaction.CommitAsync(cancellation);
        }
        catch (Exception ex)
        {
            //await transaction.RollbackAsync(cancellation);
            throw new DatabaseException($"设置划扣设计师时发生错误\n划扣Id: {consume.Id}", ex);
        }
    }

    public async ValueTask<IEnumerable<TConsume>> FindAllByYearMonthlyAsync<TConsume>(
        IIdentifiable order,
        DateTime yearMonthly,
        int? skip = null,
        int? take = null,
        CancellationToken cancellation = default) where TConsume : ConsumeViewModel
    {
        //await using var transaction = await DbContext.Database.BeginTransactionAsync(cancellation);

        try
        {
            var consumeType = typeof(TConsume);

            if (consumeType == typeof(ConsumeTimingViewModel))
            {
                var result = await LoadTiming();
                return result.Select(mapper.Map<TConsume>);
            }

            if (consumeType == typeof(ConsumeCountingViewModel))
            {
                var result = await LoadCounting();
                return result.Select(mapper.Map<TConsume>);
            }

            if (consumeType == typeof(ConsumeMixingViewModel))
            {
                var result = await LoadMixing();
                return result.Select(mapper.Map<TConsume>);
            }

            return [];
        }
        catch (Exception ex)
        {
            //await transaction.RollbackAsync(cancellation);
            throw new DatabaseException($"获取计时订单下所有划扣时发生错误\n订单Id: {order.Id}", ex);
        }

        async Task<List<ConsumeTimingEntity>> LoadTiming()
        {
            return await dbContext.ConsumeTiming
                .OrderBy(x => x.Id)
                .Where(x => x.OrderId == order.Id)
                .Where(x => x.CreateTime.Year == yearMonthly.Year && x.CreateTime.Month == yearMonthly.Month)
                .SkipAndTake(skip, take)
                .ToListAsync(cancellation);
        }

        async Task<List<ConsumeCountingEntity>> LoadCounting()
        {
            return await dbContext.ConsumeCounting
                .OrderBy(x => x.Id)
                .Where(x => x.OrderId == order.Id)
                .Where(x => x.CreateTime.Year == yearMonthly.Year && x.CreateTime.Month == yearMonthly.Month)
                .SkipAndTake(skip, take)
                .ToListAsync(cancellation);
        }

        async Task<List<ConsumeMixingEntity>> LoadMixing()
        {
            return await dbContext.ConsumeMixing
                .OrderBy(x => x.Id)
                .Where(x => x.OrderId == order.Id)
                .Where(x => x.CreateTime.Year == yearMonthly.Year && x.CreateTime.Month == yearMonthly.Month)
                .SkipAndTake(skip, take)
                .ToListAsync(cancellation);
        }
    }

    public async ValueTask<IEnumerable<TConsume>> FindAllByOrderAsync<TConsume>(
        IIdentifiable order, 
        int? skip = null, 
        int? take = null,
        CancellationToken cancellation = default) where TConsume : ConsumeViewModel
    {
        //await using var transaction = await DbContext.Database.BeginTransactionAsync(cancellation);

        try
        {
            var consumeType = typeof(TConsume);

            if (consumeType == typeof(ConsumeTimingViewModel))
            {
                var result = await LoadTiming();
                return result.Select(mapper.Map<TConsume>);
            }

            if (consumeType == typeof(ConsumeCountingViewModel))
            {
                var result = await LoadCounting();
                return result.Select(mapper.Map<TConsume>);
            }

            if (consumeType == typeof(ConsumeMixingViewModel))
            {
                var result = await LoadMixing();
                return result.Select(mapper.Map<TConsume>);
            }

            return [];
        }
        catch (Exception ex)
        {
            //await transaction.RollbackAsync(cancellation);
            throw new DatabaseException($"获取计时订单下所有划扣时发生错误\n订单Id: {order.Id}", ex);
        }


        async Task<List<ConsumeTimingEntity>> LoadTiming()
        {
            return await dbContext.ConsumeTiming
                .OrderBy(x => x.Id)
                .Where(x => x.OrderId == order.Id)
                .SkipAndTake(skip, take)
                .ToListAsync(cancellation);
        }

        async Task<List<ConsumeCountingEntity>> LoadCounting()
        {
            return await dbContext.ConsumeCounting
                .OrderBy(x => x.Id)
                .Where(x => x.OrderId == order.Id)
                .SkipAndTake(skip, take)
                .ToListAsync(cancellation);
        }

        async Task<List<ConsumeMixingEntity>> LoadMixing()
        {
            return await dbContext.ConsumeMixing
                .OrderBy(x => x.Id)
                .Where(x => x.OrderId == order.Id)
                .SkipAndTake(skip, take)
                .ToListAsync(cancellation);
        }
    }
}