using AutoMapper;
using DongKeJi.Database;
using DongKeJi.Exceptions;
using DongKeJi.Extensions;
using DongKeJi.Inject;
using DongKeJi.Validation;
using DongKeJi.Work.Model;
using DongKeJi.Work.Model.Entity.Order;
using DongKeJi.Work.Model.Entity.Staff;
using DongKeJi.Work.ViewModel.Order;
using DongKeJi.Work.ViewModel.Staff;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DongKeJi.Work.Service;

/// <summary>
///     订单服务
/// </summary>
public interface IOrderService
{
    /// <summary>
    ///     创建订单
    /// </summary>
    /// <param name="order"></param>
    /// <param name="salesperson">创建订单的销售</param>
    /// <param name="customer"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    ValueTask AddAsync(
        OrderViewModel order,
        IIdentifiable salesperson,
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
    /// 查询订单的销售
    /// </summary>
    /// <param name="order"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    ValueTask<StaffViewModel> FindSalespersonAsync(
        IIdentifiable order,
        CancellationToken cancellation = default);

    /// <summary>
    ///     更换销售
    /// </summary>
    /// <param name="order"></param>
    /// <param name="salesperson"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    ValueTask ChangeSalespersonAsync(
        IIdentifiable order,
        IIdentifiable salesperson,
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
[Inject(ServiceLifetime.Singleton, typeof(IOrderService))]
internal class OrderService(WorkDbContext dbContext, IMapper mapper) : IOrderService
{
    public async ValueTask AddAsync(
        OrderViewModel order,
        IIdentifiable salesperson,
        IIdentifiable customer,
        CancellationToken cancellation = default)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellation);

        try
        {
            //验证
            order.AssertValidate();

            //订单
            var orderEntity = await dbContext.Orders
                .FirstOrDefaultAsync(x => x.Id == order.Id, cancellationToken: cancellation);
            DatabaseException.ThrowIfEntityAlreadyExists(orderEntity, "订单已存在");


            //职位
            var positionEntity = await dbContext.StaffPositions
                .Include(x => x.Staffs)
                .FirstOrDefaultAsync(x => x.Type == StaffPositionType.Salesperson, cancellation);
            positionEntity = DatabaseException.ThrowIfEntityNotFound(positionEntity, "销售职位不存在");


            //员工
            var salespersonEntity = await dbContext.Staffs
                .Include(x => x.Positions)
                .Where(x => x.Positions.Contains(positionEntity))
                .FirstOrDefaultAsync(x => x.Id == salesperson.Id, cancellation);
            salespersonEntity = DatabaseException.ThrowIfEntityNotFound(salespersonEntity, "销售员工不存在");

            //机构
            var customerEntity = await dbContext.Customers
                .Include(x => x.Orders)
                .FirstOrDefaultAsync(x => x.Id == customer.Id, cancellation);
            customerEntity = DatabaseException.ThrowIfEntityNotFound(customerEntity, "机构不存在");

            //关联
            orderEntity = mapper.Map<OrderEntity>(order);

            orderEntity.Staffs.Add(salespersonEntity, x => x.Id != salesperson.Id);
            orderEntity.Customer = customerEntity;

            salespersonEntity.Orders.Add(orderEntity);
            customerEntity.Orders.Add(orderEntity);

            //保存
            await dbContext.AddAsync(orderEntity, cancellation);
            await dbContext.AssertSaveSuccessAsync(cancellation);
            await transaction.CommitAsync(cancellation);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellation);
            throw new DatabaseException($"添加订单时发生错误\n订单信息: {order}\n销售Id: {salesperson}\n机构Id: {customer}", ex);
        }
    }

    public async ValueTask RemoveAsync(
        IIdentifiable order,
        CancellationToken cancellation = default)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellation);

        try
        {
            //订单
            var orderEntity = await dbContext.Orders
                .FirstOrDefaultAsync(x => x.Id == order.Id, cancellationToken: cancellation);
            orderEntity = DatabaseException.ThrowIfEntityNotFound(orderEntity, "订单不存在");

            dbContext.Remove(orderEntity);

            await dbContext.AssertSaveSuccessAsync(cancellation);
            await transaction.CommitAsync(cancellation);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellation);
            throw new DatabaseException($"删除订单时发生错误\n订单Id: {order}", ex);
        }
    }

    public async ValueTask<StaffViewModel> FindSalespersonAsync(
        IIdentifiable order,
        CancellationToken cancellation = default)
    {
        //await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellation);

        try
        {
            //订单
            var orderEntity = await dbContext.Orders
                .Include(x => x.Staffs)
                .ThenInclude(staffEntity => staffEntity.Positions)
                .FirstOrDefaultAsync(x => x.Id == order.Id, cancellation);
            orderEntity = DatabaseException.ThrowIfEntityNotFound(orderEntity, "订单不存在");

            //职位
            var positionEntity = await dbContext.StaffPositions
                .FirstOrDefaultAsync(x => x.Type == StaffPositionType.Salesperson, cancellationToken: cancellation);
            positionEntity = DatabaseException.ThrowIfEntityNotFound(positionEntity, "销售职位不存在");

            //订单-销售
            var salespersonStaff = orderEntity.Staffs
                .FirstOrDefault(x => x.Positions.Contains(positionEntity));
            salespersonStaff = DatabaseException.ThrowIfEntityNotFound(salespersonStaff, "销售不存在");

            return mapper.Map<StaffViewModel>(salespersonStaff);
        }
        catch (Exception ex)
        {
            //await transaction.RollbackAsync(cancellation);
            throw new DatabaseException($"查询订单销售时发生错误\n订单Id: {order.Id}", ex);
        }
    }

    public async ValueTask ChangeSalespersonAsync(
        IIdentifiable order,
        IIdentifiable salesperson,
        CancellationToken cancellation = default)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellation);

        try
        {
            //订单
            var orderEntity = await dbContext.Orders
                .Include(x => x.Staffs)
                .ThenInclude(x => x.Positions)
                .FirstOrDefaultAsync(x => x.Id == order.Id, cancellation);
            orderEntity = DatabaseException.ThrowIfEntityNotFound(orderEntity, "订单不存在");

            //销售
            var salespersonEntity = await dbContext.Staffs
                .Include(staffEntity => staffEntity.Orders)
                .FirstOrDefaultAsync(x => x.Id == salesperson.Id, cancellation);
            salespersonEntity = DatabaseException.ThrowIfEntityNotFound(salespersonEntity, "销售不存在");

            //订单-销售
            var orderSalespersonSet = orderEntity.Staffs
                .Where(x => x.Positions.Any(y => y.Type == StaffPositionType.Salesperson))
                .ToArray();

            foreach (var salespersonItem in orderSalespersonSet)
            {
                orderEntity.Staffs.Remove(salespersonItem);
            }

            orderEntity.Staffs.Add(salespersonEntity, x => x.Id != salespersonEntity.Id);
            salespersonEntity.Orders.Add(orderEntity, x => x.Id != orderEntity.Id);

            await dbContext.AssertSaveSuccessAsync(cancellation);
            await transaction.CommitAsync(cancellation);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellation);
            throw new DatabaseException($"查询订单销售时发生错误\n订单Id: {order}", ex);
        }
    }

    public async ValueTask<IEnumerable<OrderViewModel>> GetAllByCustomerIdAsync(
        IIdentifiable customer,
        int? skip = null,
        int? take = null,
        CancellationToken cancellation = default)
    {
        //await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellation);

        try
        {
            var orderEntityList = await dbContext.Customers
                .Include(x => x.Orders)
                .Where(x => x.Id == customer.Id)
                .Select(x => x.Orders
                    .SkipAndTake(skip, take)
                    .ToList())
                .FirstOrDefaultAsync(cancellation) ?? [];

            return orderEntityList.Select(mapper.Map<OrderViewModel>);
        }
        catch (Exception ex)
        {
            //await transaction.RollbackAsync(cancellation);
            throw new DatabaseException($"获取机构下所有订单时发生错误\n机构Id: {customer}", ex);
        }
    }

    public async ValueTask<IEnumerable<OrderViewModel>> GetAllByStaffIdAsync(
        IIdentifiable staff,
        int? skip = null,
        int? take = null,
        CancellationToken cancellation = default)
    {
        //await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellation);

        try
        {
            var orderEntityList = await dbContext.Staffs
                .Include(x => x.Orders)
                .Where(x => x.Id == staff.Id)
                .Select(x => x.Orders
                    .SkipAndTake(skip, take)
                    .ToList())
                .FirstOrDefaultAsync(cancellation) ?? [];

            return orderEntityList.Select(mapper.Map<OrderViewModel>);
        }
        catch (Exception ex)
        {
            //await transaction.RollbackAsync(cancellation);
            throw new DatabaseException($"获取员工下所有订单时发生错误\n员工Id: {staff}", ex);
        }
    }
}