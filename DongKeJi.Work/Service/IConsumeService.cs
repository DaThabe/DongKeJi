using AutoMapper;
using DongKeJi.Common;
using DongKeJi.Common.Database;
using DongKeJi.Common.Extensions;
using DongKeJi.Common.Inject;
using DongKeJi.Work.Model;
using DongKeJi.Work.Model.Entity.Consume;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ConsumeViewModel = DongKeJi.Work.ViewModel.Common.Consume.ConsumeViewModel;
using CountingConsumeViewModel = DongKeJi.Work.ViewModel.Common.Consume.CountingConsumeViewModel;
using MixingConsumeViewModel = DongKeJi.Work.ViewModel.Common.Consume.MixingConsumeViewModel;
using TimingConsumeViewModel = DongKeJi.Work.ViewModel.Common.Consume.TimingConsumeViewModel;

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
    ValueTask<bool> AddAsync(
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
    ValueTask<bool> RemoveAsync(
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
[Inject(ServiceLifetime.Singleton, typeof(IConsumeService))]
internal class ConsumeService(
    PerformanceRecordDbContext dbContext,
    ILogger<ConsumeService> logger,
    IMapper mapper
) : IConsumeService
{
    public async ValueTask<bool> AddAsync(
        ConsumeViewModel consume,
        IIdentifiable order,
        IIdentifiable staff,
        CancellationToken cancellation = default)
    {
        return await dbContext.UnitOfWorkAsync(async _ =>
            {
                var consumeEntity = await dbContext.Consumes
                    .FirstOrDefaultAsync(x => x.Id == consume.Id, cancellation);

                if (consumeEntity is not null) throw new Exception("划扣已存在");

                var staffEntity = await dbContext.Staffs
                                      .Include(x => x.Customers)
                                      .Include(x => x.Orders)
                                      .FirstOrDefaultAsync(x => x.Id == staff.Id, cancellation)
                                  ?? throw new Exception($"未查询到员工: {staff.Id}");

                //创建实体
                consumeEntity = mapper.Map<ConsumeEntity>(consume);

                //计时
                if (consumeEntity is TimingConsumeEntity timingConsumeEntity)
                {
                    var orderEntity = await dbContext.TimingOrders
                                          .Include(x => x.Staffs)
                                          .Include(x => x.Consumes)
                                          .Where(x => x.Id == order.Id)
                                          .FirstOrDefaultAsync(cancellation)
                                      ?? throw new Exception("订单不存在");

                    //员工-划扣 多对多
                    staffEntity.Consumes.Add(timingConsumeEntity);
                    timingConsumeEntity.Staff = staffEntity;

                    //员工-订单 多对多
                    staffEntity.Orders.Add(orderEntity, x => x.Id != orderEntity.Id);
                    orderEntity.Staffs.Add(staffEntity, x => x.Id != staffEntity.Id);

                    //订单-划扣 一对多
                    orderEntity.Consumes.Add(timingConsumeEntity);
                    timingConsumeEntity.Order = orderEntity;

                    //保存
                    await dbContext.AddAsync(timingConsumeEntity, cancellation);
                    var result = await dbContext.SaveChangesAsync(cancellation);
                    RegisterAutoUpdate(consume);
                    return result > 0;
                }

                //计数
                if (consumeEntity is CountingConsumeEntity countingConsumeEntity)
                {
                    var vlues = dbContext.CountingOrders.ToListAsync(cancellation);


                    var orderEntity = await dbContext.CountingOrders
                                          .Include(x => x.Staffs)
                                          .Include(x => x.Consumes)
                                          .Where(x => x.Id == order.Id)
                                          .FirstOrDefaultAsync(cancellation)
                                      ?? throw new Exception("订单不存在");

                    //员工-划扣 多对多
                    staffEntity.Consumes.Add(countingConsumeEntity);
                    countingConsumeEntity.Staff = staffEntity;

                    //员工-订单 多对多
                    staffEntity.Orders.Add(orderEntity, x => x.Id != orderEntity.Id);
                    orderEntity.Staffs.Add(staffEntity, x => x.Id != staffEntity.Id);

                    //订单-划扣 一对多
                    orderEntity.Consumes.Add(countingConsumeEntity);
                    countingConsumeEntity.Order = orderEntity;

                    //保存
                    await dbContext.AddAsync(countingConsumeEntity, cancellation);
                    var result = await dbContext.SaveChangesAsync(cancellation);
                    RegisterAutoUpdate(consume);
                    return result > 0;
                }

                //混合
                if (consumeEntity is MixingConsumeEntity mixingConsumeEntity)
                {
                    var orderEntity = await dbContext.MixingOrders
                                          .Include(x => x.Staffs)
                                          .Include(x => x.Consumes)
                                          .Where(x => x.Id == order.Id)
                                          .FirstOrDefaultAsync(cancellation)
                                      ?? throw new Exception("订单不存在");

                    //员工-划扣 多对多
                    staffEntity.Consumes.Add(mixingConsumeEntity);
                    mixingConsumeEntity.Staff = staffEntity;

                    //员工-订单 多对多
                    staffEntity.Orders.Add(orderEntity, x => x.Id != orderEntity.Id);
                    orderEntity.Staffs.Add(staffEntity, x => x.Id != staffEntity.Id);

                    //订单-划扣 一对多
                    orderEntity.Consumes.Add(mixingConsumeEntity);
                    mixingConsumeEntity.Order = orderEntity;

                    //保存
                    await dbContext.AddAsync(mixingConsumeEntity, cancellation);
                    var result = await dbContext.SaveChangesAsync(cancellation);
                    RegisterAutoUpdate(consume);

                    return result > 0;
                }

                return false;
            },
            ex => logger.LogError(ex, "添加划扣且关联订单和员工时发生错误, 划扣Id: {consume}, 订单Id:{order}, 员工Id:{staff}", consume.Id,
                order.Id, staff.Id));
    }

    public async ValueTask<bool> RemoveAsync(
        IIdentifiable consume,
        CancellationToken cancellation = default)
    {
        return await dbContext.UnitOfWorkAsync(async transaction =>
        {
            var consumeEntity = await dbContext.Consumes
                                    .FirstOrDefaultAsync(x => x.Id == consume.Id, cancellation)
                                ?? throw new Exception("划扣不存在");

            dbContext.Remove(consumeEntity);
            var result = await dbContext.SaveChangesAsync(cancellation);
            return result > 0;
        }, ex => logger.LogError(ex, "删除划扣时发生错误, 划扣Id:{consume}", consume.Id));
    }

    public async ValueTask<IEnumerable<TimingConsumeViewModel>> GetAllByTimingOrderAsync(
        IIdentifiable order,
        int? skip = null,
        int? take = null,
        CancellationToken cancellation = default)
    {
        return await dbContext.UnitOfWorkAsync(async _ =>
        {
            var entityList = await dbContext.TimingConsumes
                .OrderBy(x => x.Id)
                .Where(x => x.OrderId == order.Id)
                .SkipAndTake(skip, take)
                .ToListAsync(cancellation);

            return entityList.Select(RegisterAutoUpdate<TimingConsumeViewModel>).ToArray();
        }, ex => logger.LogError(ex, "获取计时订单所有划扣时发生错误, 订单Id:{order}", order.Id)) ?? [];
    }

    public async ValueTask<IEnumerable<CountingConsumeViewModel>> GetAllByCountingOrderAsync(
        IIdentifiable order,
        int? skip = null,
        int? take = null,
        CancellationToken cancellation = default)
    {
        return await dbContext.UnitOfWorkAsync(async _ =>
        {
            var entityList = await dbContext.CountingConsumes
                .OrderBy(x => x.Id)
                .Where(x => x.OrderId == order.Id)
                .SkipAndTake(skip, take)
                .ToListAsync(cancellation);

            return entityList.Select(RegisterAutoUpdate<CountingConsumeViewModel>).ToArray();
        }, ex => logger.LogError(ex, "获取计数订单所有划扣时发生错误, 订单Id:{order}", order.Id)) ?? [];
    }

    public async ValueTask<IEnumerable<MixingConsumeViewModel>> GetAllByMixingOrderAsync(
        IIdentifiable order,
        int? skip = null,
        int? take = null,
        CancellationToken cancellation = default)
    {
        return await dbContext.UnitOfWorkAsync(async _ =>
        {
            var entityList = await dbContext.MixingConsumes
                .OrderBy(x => x.Id)
                .Where(x => x.OrderId == order.Id)
                .SkipAndTake(skip, take)
                .ToListAsync(cancellation);

            return entityList.Select(RegisterAutoUpdate<MixingConsumeViewModel>);
        }, ex => logger.LogError(ex, "获取混合订单所有划扣时发生错误, 订单Id:{order}", order.Id)) ?? [];
    }


    protected TConsumeViewModel RegisterAutoUpdate<TConsumeViewModel>(TConsumeViewModel vm)
        where TConsumeViewModel : ConsumeViewModel
    {
        vm.PropertyChanged += async (sender, e) =>
        {
            var existEntity = await dbContext.Consumes
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

    protected TConsumeViewModel RegisterAutoUpdate<TConsumeViewModel>(ConsumeEntity entity)
        where TConsumeViewModel : ConsumeViewModel
    {
        var vm = mapper.Map<TConsumeViewModel>(entity);
        return RegisterAutoUpdate(vm);
    }
}