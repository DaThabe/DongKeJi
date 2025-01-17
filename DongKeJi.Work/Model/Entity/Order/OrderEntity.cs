using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DongKeJi.Entity;
using DongKeJi.Work.Model.Entity.Customer;
using DongKeJi.Work.Model.Entity.Staff;

namespace DongKeJi.Work.Model.Entity.Order;

/// <summary>
///     订单
/// </summary>
[Table("Order")]
internal abstract class OrderEntity : EntityBase
{
    /// <summary>
    ///     名称
    /// </summary>
    [Required]
    [Column("Name")]
    public required string Name { get; set; }

    /// <summary>
    ///     名称
    /// </summary>
    [Required]
    [Column("Price")]
    public required double Price { get; set; }

    /// <summary>
    ///     描述
    /// </summary>
    [Column("Describe")]
    public string? Describe { get; set; }

    /// <summary>
    ///     订阅时间
    /// </summary>
    [Required]
    [Column("SubscribeTime")]
    public required DateTime SubscribeTime { get; set; }

    /// <summary>
    ///     状态
    /// </summary>
    [Required]
    [Column("State")]
    public required OrderState State { get; set; }


    /// <summary>
    ///     订单所属机构Id
    /// </summary>
    [Column("CustomerId")]
    public required Guid CustomerId { get; set; }

    /// <summary>
    ///     订单所属机构
    /// </summary>
    [ForeignKey(nameof(CustomerId))]
    public CustomerEntity Customer { get; set; } = null!;

    /// <summary>
    ///     所有关联的员工
    /// </summary>
    public ICollection<StaffEntity> Staffs { get; set; } = [];
}