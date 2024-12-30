using DongKeJi.Common;
using DongKeJi.Common.Exceptions;
using DongKeJi.Common.Extensions;
using DongKeJi.Common.Inject;
using DongKeJi.Common.Service;
using DongKeJi.Work.Model;
using DongKeJi.Work.Model.Entity.Customer;
using DongKeJi.Work.ViewModel.Common.Customer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DongKeJi.Work.Service;

public interface ICustomerRepository
{
    /// <summary>
    ///     添加机构且关联用户
    /// </summary>
    /// <param name="customer"></param>
    /// <param name="staff"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    ValueTask AddAsync(
        CustomerViewModel customer,
        IIdentifiable staff,
        CancellationToken cancellation = default);

    /// <summary>
    ///     删除机构
    /// </summary>
    /// <param name="customer"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    ValueTask RemoveAsync(
        IIdentifiable customer,
        CancellationToken cancellation = default);

    /// <summary>
    ///     查找某个员工关联的机构
    /// </summary>
    /// <param name="staff"></param>
    /// <param name="take"></param>
    /// <param name="cancellation"></param>
    /// <param name="skip"></param>
    ValueTask<IEnumerable<CustomerViewModel>> GetAllByStaffIdAsync(
        IIdentifiable staff,
        int? skip = null,
        int? take = null,
        CancellationToken cancellation = default);
}

[Inject(ServiceLifetime.Singleton, typeof(ICustomerRepository))]
internal class CustomerRepository(IServiceProvider services) : 
    Repository<WorkDbContext, CustomerEntity, CustomerViewModel>(services), ICustomerRepository
{
    public ValueTask AddAsync(
        CustomerViewModel customer,
        IIdentifiable staff,
        CancellationToken cancellation = default)
    {
        return UnitOfWorkAsync(async _ =>
        {
            var customerEntity = await DbContext.Customers
                .FirstOrDefaultAsync(x => x.Id == customer.Id, cancellation);

            if (customerEntity is not null)
            {
                throw new RepositoryException($"机构添加失败, 相同Id已存在\n机构信息: {customer}");
            }

            var staffEntity = await DbContext.Staffs
                .Include(x => x.Customers)
                .FirstOrDefaultAsync(x => x.Id == staff.Id, cancellation);
            
            if (staffEntity is null || staffEntity.IsEmpty())
            {
                throw new RepositoryException($"机构添加失败, 关联员工不存在\n机构信息: {customer}\n员工Id: {staff.Id}");
            }

            //编辑
            customerEntity = Mapper.Map<CustomerEntity>(customer);
            await DbContext.AddAsync(customerEntity, cancellation);

            //关联
            customerEntity.Staffs.Add(staffEntity, x => x.Id == staff.Id);
            staffEntity.Customers.Add(customerEntity, x => x.Id == customer.Id);

            //保存
            if (await DbContext.SaveChangesAsync(cancellation) <= 0)
            {
                throw new RepositoryException($"机构添加失败, 未写入数据库\n机构信息: {customer}\n员工Id: {staff.Id}");
            }
            RegisterAutoUpdate(customer);

        }, cancellation);
    }

    public ValueTask RemoveAsync(
        IIdentifiable customer,
        CancellationToken cancellation = default)
    {
        return UnitOfWorkAsync(async _ =>
        {
            var orderEntity = await DbContext.Customers
                .FirstOrDefaultAsync(x => x.Id == customer.Id, cancellation);

            if (orderEntity is null || orderEntity.IsEmpty())
            {
                throw new RepositoryException($"划扣删除失败, 数据不存在\n机构Id: {customer.Id}");
            }

            DbContext.Remove(orderEntity);
            if (await DbContext.SaveChangesAsync(cancellation) <= 0)
            {
                throw new RepositoryException($"机构删除失败, 未写入数据库\n机构Id: {customer.Id}");
            }
        }, cancellation);
    }


    public ValueTask<IEnumerable<CustomerViewModel>> GetAllByStaffIdAsync(
        IIdentifiable staff,
        int? skip = null,
        int? take = null,
        CancellationToken cancellation = default)
    {
        return UnitOfWorkAsync(async _ =>
        {
            var customerEntityList = await DbContext.Staffs
                .Include(x => x.Customers)
                .Where(x => x.Id == staff.Id)
                .Select(x => x.Customers
                    .SkipAndTake(skip, take)
                    .ToList())
                .FirstOrDefaultAsync(cancellation);

            return customerEntityList?.Select(x => RegisterAutoUpdate(x)) ?? [];

        }, cancellation);
    }
}