using DongKeJi.Config;
using DongKeJi.Database;
using DongKeJi.Entity;
using DongKeJi.Work.Model.Entity.Consume;
using DongKeJi.Work.Model.Entity.Customer;
using DongKeJi.Work.Model.Entity.Order;
using DongKeJi.Work.Model.Entity.Staff;
using Microsoft.EntityFrameworkCore;

namespace DongKeJi.Work.Model;

internal class WorkDbContext(IApplication applicationContext) : LocalDbContext(applicationContext, "PerformanceRecord"), IConfigDbContext
{
    /// <summary>
    /// 配置
    /// </summary>
    public DbSet<ConfigEntity> Configs { get; set; }


    /// <summary>
    ///     员工
    /// </summary>
    public DbSet<StaffEntity> Staffs { get; set; }

    /// <summary>
    ///     员工职位
    /// </summary>
    public DbSet<StaffPositionEntity> StaffPositions { get; set; }

    /// <summary>
    ///     机构
    /// </summary>
    public DbSet<CustomerEntity> Customers { get; set; }


    /// <summary>
    ///     所有订单
    /// </summary>
    public DbSet<OrderEntity> Orders { get; set; }

    /// <summary>
    ///     计时订单
    /// </summary>
    public DbSet<OrderTimingEntity> TimingOrders { get; set; }

    /// <summary>
    ///     计数订单
    /// </summary>
    public DbSet<OrderCountingEntity> CountingOrders { get; set; }

    /// <summary>
    ///     混合订单
    /// </summary>
    public DbSet<OrderMixingEntity> MixingOrders { get; set; }


    /// <summary>
    ///     所有划扣
    /// </summary>
    public DbSet<ConsumeEntity> Consumes { get; set; }

    /// <summary>
    ///     计时订单划扣
    /// </summary>
    public DbSet<ConsumeTimingEntity> TimingConsumes { get; set; }

    /// <summary>
    ///     计数订单划扣
    /// </summary>
    public DbSet<ConsumeCountingEntity> CountingConsumes { get; set; }

    /// <summary>
    ///     混合订单划扣
    /// </summary>
    public DbSet<ConsumeMixingEntity> MixingConsumes { get; set; }




    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(WorkDbContext).Assembly);
    }
}