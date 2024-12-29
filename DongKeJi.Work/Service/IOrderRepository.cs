using DongKeJi.Common;
using DongKeJi.Common.Exceptions;
using DongKeJi.Common.Extensions;
using DongKeJi.Common.Inject;
using DongKeJi.Common.Service;
using DongKeJi.Work.Model;
using DongKeJi.Work.Model.Entity.Order;
using DongKeJi.Work.ViewModel.Common.Order;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

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
    ValueTask AddAsync(
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
    ValueTask RemoveAsync(
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
internal class OrderRepository(IServiceProvider services) :
    Repository<WorkDbContext, OrderEntity, OrderViewModel>(services), IOrderRepository
{
    public ValueTask AddAsync(
        OrderViewModel order,
        IIdentifiable staff,
        IIdentifiable customer,
        CancellationToken cancellation = default)
    {
        return UnitOfWorkAsync(async _ =>
        {
            var orderEntity = await DbContext.Orders
                .FirstOrDefaultAsync(x => x.Id == order.Id, cancellation);

            if (orderEntity is not null)
            {
                throw new RepositoryException($"订单添加失败, 订单已存在\n订单信息: {order}\n员工Id: {staff.Id}, 机构Id: {customer}");
            }


            var staffEntity = await DbContext.Staffs
                .Include(x => x.Orders)
                .Include(x => x.Customers)
                .FirstOrDefaultAsync(x => x.Id == staff.Id, cancellation);

            if (staffEntity is null || staffEntity.IsEmpty())
            {
                throw new RepositoryException($"订单添加失败, 关联员工不存在\n订单信息: {order}\n员工Id: {staff.Id}");
            }

            var customerEntity = await DbContext.Customers
                .Include(x => x.Orders)
                .Include(x => x.Staffs)
                .FirstOrDefaultAsync(x => x.Id == customer.Id, cancellation);
            
            if (customerEntity is null || customerEntity.IsEmpty())
            {
                throw new RepositoryException($"订单添加失败, 关联机构不存在\n订单信息: {order}\n机构Id: {customer.Id}");
            }

            //创建实体
            orderEntity = Mapper.Map<OrderEntity>(order);

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
            await DbContext.AddAsync(orderEntity, cancellation);
            if(await DbContext.SaveChangesAsync(cancellation) <= 0)
            {
                throw new RepositoryException($"订单添加失败, 未写入数据库\n订单信息: {order}\n员工Id: {staff.Id}\n机构Id: {customer.Id}");
            }
            
            RegisterAutoUpdate(order);

        }, cancellation);
    }

    public ValueTask RemoveAsync(
        IIdentifiable order,
        CancellationToken cancellation = default)
    {
        return UnitOfWorkAsync(async _ =>
        {
            var orderEntity = await DbContext.Orders
                .FirstOrDefaultAsync(x => x.Id == order.Id, cancellation);

            if (orderEntity is null || orderEntity.IsEmpty())
            {
                throw new RepositoryException($"机构删除失败, 数据不存在\n订单Id: {order.Id}");
            }

            DbContext.Remove(orderEntity);
            if (await DbContext.SaveChangesAsync(cancellation) <= 0)
            {
                throw new RepositoryException($"机构删除失败, 未写入数据库\n订单Id: {orderEntity.Id}");
            }

        }, cancellation);
    }

    public ValueTask<IEnumerable<OrderViewModel>> GetAllByCustomerIdAsync(
        IIdentifiable customer,
        int? skip = null,
        int? take = null,
        CancellationToken cancellation = default)
    {
        return UnitOfWorkAsync(async _ =>
        {
            var orderEntityList = await DbContext.Customers
                .Include(x => x.Orders)
                .Where(x => x.Id == customer.Id)
                .Select(x => x.Orders
                    .SkipAndTake(skip, take)
                    .ToList())
                .FirstOrDefaultAsync(cancellation);

            return orderEntityList?.Select(RegisterAutoUpdate<OrderViewModel>) ?? [];

        }, cancellation);
    }

    public ValueTask<IEnumerable<OrderViewModel>> GetAllByStaffIdAsync(
        IIdentifiable staff,
        int? skip = null,
        int? take = null,
        CancellationToken cancellation = default)
    {
        return UnitOfWorkAsync(async _ =>
        {
            var orderEntityList = await DbContext.Staffs
                .Include(x => x.Orders)
                .Where(x => x.Id == staff.Id)
                .Select(x => x.Orders
                    .SkipAndTake(skip, take)
                    .ToList())
                .FirstOrDefaultAsync(cancellation);

            return orderEntityList?.Select(RegisterAutoUpdate<OrderViewModel>) ?? [];

        }, cancellation);
    }
}