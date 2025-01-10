using AutoMapper;
using DongKeJi.Common;
using DongKeJi.Common.Exceptions;
using DongKeJi.Common.Extensions;
using DongKeJi.Common.Inject;
using DongKeJi.Common.Service;
using DongKeJi.Common.Validation;
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
internal class CustomerRepository(
    IServiceProvider services,
    IMapper mapper,
    WorkDbContext dbContext) : ICustomerRepository
{
    public async ValueTask AddAsync(
        CustomerViewModel customer,
        IIdentifiable staff,
        CancellationToken cancellation = default)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellation);

        try
        {
            //验证
            customer.AssertValidate();

            //机构
            var customerEntity = await dbContext.Customers
                .FirstOrDefaultAsync(x => x.Id == customer.Id, cancellation);
            RepositoryException.ThrowIfEntityAlreadyExists(customerEntity, "机构已存在");

            //员工
            var staffEntity = await dbContext.Staffs
                .Include(x => x.Customers)
                .FirstOrDefaultAsync(x => x.Id == staff.Id, cancellation);
            staffEntity = RepositoryException.ThrowIfEntityNotFound(staffEntity, "关联员工不存在");

            //编辑
            customerEntity = mapper.Map<CustomerEntity>(customer);
            await dbContext.AddAsync(customerEntity, cancellation);

            //关联
            customerEntity.Staffs.Add(staffEntity, x => x.Id == staff.Id);
            staffEntity.Customers.Add(customerEntity, x => x.Id == customer.Id);

            //保存
            await dbContext.AssertSaveSuccessAsync(cancellation: cancellation);
            await transaction.CommitAsync(cancellation);

            dbContext.RegisterAutoUpdate<CustomerEntity, CustomerViewModel>(customer, services);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellation);
            throw new RepositoryException($"添加机构时发生错误\n机构信息: {customer}\n员工Id: {staff.Id}", ex);
        }
    }

    public async ValueTask RemoveAsync(
        IIdentifiable customer,
        CancellationToken cancellation = default)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellation);

        try
        {
            var orderEntity = await dbContext.Customers
                .FirstOrDefaultAsync(x => x.Id == customer.Id, cancellation);
            orderEntity = RepositoryException.ThrowIfEntityNotFound(orderEntity, "机构不存在");

            dbContext.Remove(orderEntity);

            await dbContext.AssertSaveSuccessAsync(cancellation);
            await transaction.CommitAsync(cancellation);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellation);
            throw new RepositoryException($"删除机构时发生错误\n机构信息: {customer}", ex);
        }
    }


    public async ValueTask<IEnumerable<CustomerViewModel>> GetAllByStaffIdAsync(
        IIdentifiable staff,
        int? skip = null,
        int? take = null,
        CancellationToken cancellation = default)
    {
        //await using var transaction = await DbContext.Database.BeginTransactionAsync(cancellation);

        try
        {
            var customerEntityList = await dbContext.Staffs
                .Include(x => x.Customers)
                .Where(x => x.Id == staff.Id)
                .Select(x => x.Customers
                    .SkipAndTake(skip, take)
                    .ToList())
                .FirstOrDefaultAsync(cancellation) ?? [];

            return customerEntityList.Select(x =>
                dbContext.RegisterAutoUpdate<CustomerEntity, CustomerViewModel>(x, services));
        }
        catch (Exception ex)
        {
            //await transaction.RollbackAsync(cancellation);
            throw new RepositoryException($"获取员工下所有机构时发生错误\n员工Id: {staff.Id}", ex);
        }
    }
}