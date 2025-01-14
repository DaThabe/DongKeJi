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
    public DbSet<ConfigEntity> Config { get; set; }


    /// <summary>
    ///     员工
    /// </summary>
    public DbSet<StaffEntity> Staff { get; set; }

    /// <summary>
    ///     员工职位
    /// </summary>
    public DbSet<StaffPositionEntity> StaffPosition { get; set; }

    /// <summary>
    ///     机构
    /// </summary>
    public DbSet<CustomerEntity> Customer { get; set; }


    /// <summary>
    ///     所有订单
    /// </summary>
    public DbSet<OrderEntity> Order { get; set; }

    /// <summary>
    ///     计时订单
    /// </summary>
    public DbSet<OrderTimingEntity> OrderTiming { get; set; }

    /// <summary>
    ///     计数订单
    /// </summary>
    public DbSet<OrderCountingEntity> OrderCounting { get; set; }

    /// <summary>
    ///     混合订单
    /// </summary>
    public DbSet<OrderMixingEntity> OrderMixing { get; set; }


    /// <summary>
    ///     所有划扣
    /// </summary>
    public DbSet<ConsumeEntity> Consume { get; set; }

    /// <summary>
    ///     计时订单划扣
    /// </summary>
    public DbSet<ConsumeTimingEntity> ConsumeTiming { get; set; }

    /// <summary>
    ///     计数订单划扣
    /// </summary>
    public DbSet<ConsumeCountingEntity> ConsumeCounting { get; set; }

    /// <summary>
    ///     混合订单划扣
    /// </summary>
    public DbSet<ConsumeMixingEntity> ConsumeMixing { get; set; }




    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(WorkDbContext).Assembly);
    }
}