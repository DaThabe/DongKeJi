using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DongKeJi.Work.Model.Entity.Order;

namespace DongKeJi.Work.Model.Entity.Consume;

/// <summary>
///     计时机构划扣
/// </summary>
[Table("ConsumeTiming")]
internal class TimingConsumeEntity : ConsumeEntity
{
    /// <summary>
    ///     划扣天数
    /// </summary>
    [Required]
    [Column("ConsumeDays")]
    public required double ConsumeDays { get; set; }


    /// <summary>
    ///     划扣所属订单Id
    /// </summary>
    [Column("OrderId")]
    public required Guid OrderId { get; set; }

    /// <summary>
    ///     划扣所属订单
    /// </summary>
    [ForeignKey(nameof(OrderId))]
    public TimingOrderEntity Order { get; set; } = null!;
}