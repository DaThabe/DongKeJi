using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DongKeJi.Entity;
using DongKeJi.Work.Model.Entity.Order;
using DongKeJi.Work.Model.Entity.Staff;

namespace DongKeJi.Work.Model.Entity.Customer;

/// <summary>
///     机构信息
/// </summary>
[Table("Customer")]
internal class CustomerEntity : EntityBase
{
    /// <summary>
    ///     名称
    /// </summary>
    [Required]
    [Column("Name")]
    public required string Name { get; set; }

    /// <summary>
    ///     所在地区
    /// </summary>
    [Column("Area")]
    public string? Area { get; set; }


    /// <summary>
    ///     所有订单
    /// </summary>
    public ICollection<OrderEntity> Orders { get; set; } = [];

    /// <summary>
    ///     所有关联的员工
    /// </summary>
    public ICollection<StaffEntity> Staffs { get; set; } = [];
}