using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DongKeJi.Entity;
using DongKeJi.Work.Model.Entity.Consume;
using DongKeJi.Work.Model.Entity.Customer;
using DongKeJi.Work.Model.Entity.Order;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DongKeJi.Work.Model.Entity.Staff;

/// <summary>
///     员工
/// </summary>
[Table("Staff")]
internal class StaffEntity : EntityBase
{
    /// <summary>
    ///     用户 Guid
    /// </summary>
    [Column("UserId")]
    public Guid? UserId { get; set; }

    /// <summary>
    ///     名称
    /// </summary>
    [Required]
    [Column("Name")]
    public required string Name { get; set; }

    /// <summary>
    ///     是否是主账户
    /// </summary>
    [Required]
    [Column("IsPrimaryAccount")]
    public required bool IsPrimaryAccount { get; set; }


    /// <summary>
    ///     所有职位
    /// </summary>
    public ICollection<StaffPositionEntity> Positions { get; set; } = [];

    /// <summary>
    ///     所有关联的机构
    /// </summary>
    public ICollection<CustomerEntity> Customers { get; set; } = [];

    /// <summary>
    ///     所有关联的机构
    /// </summary>
    public ICollection<OrderEntity> Orders { get; set; } = [];

    /// <summary>
    ///     所有关联的机构
    /// </summary>
    public ICollection<ConsumeEntity> Consumes { get; set; } = [];
}

internal class StaffEntityConfiguration : IEntityTypeConfiguration<StaffEntity>
{
    public void Configure(EntityTypeBuilder<StaffEntity> builder)
    {
        builder
            .HasMany(x => x.Orders)
            .WithMany(x => x.Staffs)
            .UsingEntity("LinkStaffOrder");

        builder
            .HasMany(x => x.Positions)
            .WithMany(x => x.Staffs)
            .UsingEntity("LinkStaffPosition");

        builder
            .HasMany(x => x.Customers)
            .WithMany(x => x.Staffs)
            .UsingEntity("LinkStaffCustomer");
    }
}