using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DongKeJi.Work.Model.Entity.Order;

namespace DongKeJi.Work.Model.Entity.Consume;

/// <summary>
///     计数机构划扣
/// </summary>
[Table("ConsumeCounting")]
internal class ConsumeCountingEntity : ConsumeEntity
{
    /// <summary>
    ///     划扣数量
    /// </summary>
    [Required]
    [Column("consume_counts")]
    public required double ConsumeCounts { get; set; }


    /// <summary>
    ///     划扣所属订单Id
    /// </summary>
    [Column("OrderId")]
    public required Guid OrderId { get; init; }

    /// <summary>
    ///     划扣所属订单
    /// </summary>
    [ForeignKey(nameof(OrderId))]
    public virtual OrderCountingEntity Order { get; set; } = null!;
}