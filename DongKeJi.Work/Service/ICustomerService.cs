using AutoMapper;
using DongKeJi.Common;
using DongKeJi.Common.Database;
using DongKeJi.Common.Extensions;
using DongKeJi.Common.Inject;
using DongKeJi.Work.Model;
using DongKeJi.Work.Model.Entity.Customer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CustomerViewModel = DongKeJi.Work.ViewModel.Common.Customer.CustomerViewModel;

namespace DongKeJi.Work.Service;

public interface ICustomerService
{
    /// <summary>
    ///     添加机构且关联用户
    /// </summary>
    /// <param name="customer"></param>
    /// <param name="staff"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    ValueTask<bool> AddAsync(
        CustomerViewModel customer,
        IIdentifiable staff,
        CancellationToken cancellation = default);

    /// <summary>
    ///     删除机构
    /// </summary>
    /// <param name="customer"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    ValueTask<bool> RemoveAsync(
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

[Inject(ServiceLifetime.Singleton, typeof(ICustomerService))]
internal class CustomerService(
    PerformanceRecordDbContext dbContext,
    ILogger<CustomerService> logger,
    IMapper mapper
) : ICustomerService
{
    public async ValueTask<bool> AddAsync(
        CustomerViewModel customer,
        IIdentifiable staff,
        CancellationToken cancellation = default)
    {
        return await dbContext.UnitOfWorkAsync(async _ =>
        {
            var customerEntity = await dbContext.Customers
                .FirstOrDefaultAsync(x => x.Id == customer.Id, cancellation);

            if (customerEntity is not null) throw new Exception("机构已存在");

            var staffEntity = await dbContext.Staffs
                                  .Include(x => x.Customers)
                                  .FirstOrDefaultAsync(x => x.Id == staff.Id, cancellation)
                              ?? throw new Exception($"未查询到员工: {staff.Id}");


            //编辑
            customerEntity = mapper.Map<CustomerEntity>(customer);
            await dbContext.AddAsync(customerEntity, cancellation);

            //关联
            customerEntity.Staffs.Add(staffEntity, x => x.Id == staff.Id);
            staffEntity.Customers.Add(customerEntity, x => x.Id == customer.Id);

            //保存
            var result = await dbContext.SaveChangesAsync(cancellation);
            RegisterAutoUpdate(customer);
            return result > 0;
        }, ex => logger.LogError(ex, "添加机构且关联员工时发生错误, 机构Id:{customer}, 员工Id:{staff}", customer.Id, staff.Id));
    }

    public async ValueTask<bool> RemoveAsync(
        IIdentifiable customer,
        CancellationToken cancellation = default)
    {
        return await dbContext.UnitOfWorkAsync(async transaction =>
        {
            var orderEntity = await dbContext.Customers
                                  .FirstOrDefaultAsync(x => x.Id == customer.Id, cancellation)
                              ?? throw new Exception("机构不存在");

            dbContext.Remove(orderEntity);
            var result = await dbContext.SaveChangesAsync(cancellation);
            return result > 0;
        }, ex => logger.LogError(ex, "删除机构时发生错误, 机构Id:{customer}", customer.Id));
    }


    public async ValueTask<IEnumerable<CustomerViewModel>> GetAllByStaffIdAsync(
        IIdentifiable staff,
        int? skip = null,
        int? take = null,
        CancellationToken cancellation = default)
    {
        return await dbContext.UnitOfWorkAsync(async _ =>
        {
            var customerEntityList = await dbContext.Staffs
                                         .Include(x => x.Customers)
                                         .Where(x => x.Id == staff.Id)
                                         .Select(x => x.Customers
                                             .SkipAndTake(skip, take)
                                             .ToList())
                                         .FirstOrDefaultAsync(cancellation)
                                     ?? throw new Exception("员工不存在");

            return customerEntityList.Select(RegisterAutoUpdate);
        }, ex => logger.LogError(ex, "查询指定员工关联的所有机构时发生错误, 员工Id:{staff}", staff.Id)) ?? [];
    }


    protected CustomerViewModel RegisterAutoUpdate(CustomerViewModel vm)
    {
        vm.PropertyChanged += async (sender, e) =>
        {
            var existEntity = await dbContext.Customers
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


    protected CustomerViewModel RegisterAutoUpdate(CustomerEntity entity)
    {
        var vm = mapper.Map<CustomerViewModel>(entity);
        return RegisterAutoUpdate(vm);
    }
}