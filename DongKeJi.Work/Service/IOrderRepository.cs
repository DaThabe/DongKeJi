using AutoMapper;
using DongKeJi.Common;
using DongKeJi.Common.Database;
using DongKeJi.Common.Extensions;
using DongKeJi.Common.Inject;
using DongKeJi.Work.Model;
using DongKeJi.Work.Model.Entity.Order;
using DongKeJi.Work.ViewModel.Common.Order;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DongKeJi.Work.Service;

/// <summary>
///     订单服务
/// </summary>
public interface IOrderRepository
{
    /// <summary>
    ///     创建订单
    /// </summary>
    /// <param name="order"></param>
    /// <param name="staff">创建订单的员工</param>
    /// <param name="customer"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    ValueTask<bool> AddAsync(
        OrderViewModel order,
        IIdentifiable staff,
        IIdentifiable customer,
        CancellationToken cancellation = default);

    /// <summary>
    ///     删除订单
    /// </summary>
    /// <param name="order"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    ValueTask<bool> RemoveAsync(
        IIdentifiable order,
        CancellationToken cancellation = default);


    /// <summary>
    ///     获取机构关联的所有订单
    /// </summary>
    /// <param name="customer"></param>
    /// <param name="skip"></param>
    /// <param name="take"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    ValueTask<IEnumerable<OrderViewModel>> GetAllByCustomerIdAsync(
        IIdentifiable customer,
        int? skip = null,
        int? take = null,
        CancellationToken cancellation = default);

    /// <summary>
    ///     根据员工Id查询所有订单
    /// </summary>
    /// <param name="staff"></param>
    /// <param name="skip"></param>
    /// <param name="take"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    ValueTask<IEnumerable<OrderViewModel>> GetAllByStaffIdAsync(
        IIdentifiable staff,
        int? skip = null,
        int? take = null,
        CancellationToken cancellation = default);
}

/// <summary>
///     订单服务
/// </summary>
[Inject(ServiceLifetime.Singleton, typeof(IOrderRepository))]
internal class OrderRepository(
    ILogger<OrderRepository> logger,
    IMapper mapper,
    WorkDbContext dbContext
) : IOrderRepository
{
    public async ValueTask<bool> AddAsync(
        OrderViewModel order,
        IIdentifiable staff,
        IIdentifiable customer,
        CancellationToken cancellation = default)
    {
        return await dbContext.UnitOfWorkAsync(async transaction =>
            {
                var orderEntity = await dbContext.Orders
                    .FirstOrDefaultAsync(x => x.Id == order.Id, cancellation);

                if (orderEntity is not null) throw new Exception("订单已存在, 无法添加");


                var staffEntity = await dbContext.Staffs
                                      .Include(x => x.Orders)
                                      .Include(x => x.Customers)
                                      .FirstOrDefaultAsync(x => x.Id == staff.Id, cancellation)
                                  ?? throw new Exception($"未查询到员工: {staff.Id}");

                var customerEntity = await dbContext.Customers
                                         .Include(x => x.Orders)
                                         .Include(x => x.Staffs)
                                         .FirstOrDefaultAsync(x => x.Id == customer.Id, cancellation)
                                     ?? throw new Exception($"未查询到机构: {customer.Id}");

                //创建实体
                orderEntity = mapper.Map<OrderEntity>(order);

                //员工-机构 多对多
                staffEntity.Customers.Add(customerEntity, x => x.Id != customerEntity.Id);
                customerEntity.Staffs.Add(staffEntity, x => x.Id != staffEntity.Id);

                //订单-员工 多对多
                orderEntity.Staffs.Add(staffEntity);
                staffEntity.Orders.Add(orderEntity);

                //订单-机构 多对多
                orderEntity.Customer = customerEntity;
                customerEntity.Orders.Add(orderEntity);

                //保存
                await dbContext.AddAsync(orderEntity, cancellation);
                var result = await dbContext.SaveChangesAsync(cancellation);
                RegisterAutoUpdate(order);
                return result > 0;
            },
            ex => logger.LogError(ex, "创建订单时发生错误, 订单Id:{order}, 员工Id:{staff}, 机构Id:{customer}",
                order.Id, staff.Id, customer.Id));
    }

    public async ValueTask<bool> RemoveAsync(
        IIdentifiable order,
        CancellationToken cancellation = default)
    {
        return await dbContext.UnitOfWorkAsync(async transaction =>
        {
            var orderEntity = await dbContext.Orders
                                  .FirstOrDefaultAsync(x => x.Id == order.Id, cancellation)
                              ?? throw new Exception("订单不存在");

            dbContext.Remove(orderEntity);
            var result = await dbContext.SaveChangesAsync(cancellation);

            return result > 0;
        }, ex => logger.LogError(ex, "删除订单时发生错误, 订单Id:{order}", order.Id));
    }

    public async ValueTask<IEnumerable<OrderViewModel>> GetAllByCustomerIdAsync(
        IIdentifiable customer,
        int? skip = null,
        int? take = null,
        CancellationToken cancellation = default)
    {
        return await dbContext.UnitOfWorkAsync(async transaction =>
        {
            var orderEntityList = await dbContext.Customers
                                      .Include(x => x.Orders)
                                      .Where(x => x.Id == customer.Id)
                                      .Select(x => x.Orders
                                          .SkipAndTake(skip, take)
                                          .ToList())
                                      .FirstOrDefaultAsync(cancellation)
                                  ?? throw new Exception("所查机构不存在");

            return orderEntityList.Select(RegisterAutoUpdate<OrderViewModel>);
        }, ex => logger.LogError(ex, "获取机构关联的所有订单时发生错误, 机构Id:{customer}", customer.Id)) ?? [];
    }


    public async ValueTask<IEnumerable<OrderViewModel>> GetAllByStaffIdAsync(
        IIdentifiable staff,
        int? skip = null,
        int? take = null,
        CancellationToken cancellation = default)
    {
        return await dbContext.UnitOfWorkAsync(async transaction =>
        {
            var orderEntityList = await dbContext.Staffs
                                      .Include(x => x.Orders)
                                      .Where(x => x.Id == staff.Id)
                                      .Select(x => x.Orders
                                          .SkipAndTake(skip, take)
                                          .ToList())
                                      .FirstOrDefaultAsync(cancellation)
                                  ?? throw new Exception("所查询员工不存在");

            return orderEntityList.Select(RegisterAutoUpdate<OrderViewModel>);
        }, ex => logger.LogError(ex, "获取员工关联的所有订单时发生错误, 员工Id:{staff}", staff.Id)) ?? [];
    }


    protected TOrderViewModel RegisterAutoUpdate<TOrderViewModel>(TOrderViewModel vm)
        where TOrderViewModel : OrderViewModel
    {
        vm.PropertyChanged += async (sender, e) =>
        {
            var existEntity = await dbContext.Orders
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

    protected TOrderViewModel RegisterAutoUpdate<TOrderViewModel>(OrderEntity entity)
        where TOrderViewModel : OrderViewModel
    {
        var vm = mapper.Map<TOrderViewModel>(entity);
        return RegisterAutoUpdate(vm);
    }
}