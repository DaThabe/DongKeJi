using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DongKeJi.Work.Model.Entity.Consume;

namespace DongKeJi.Work.Model.Entity.Order;

/// <summary>
///     计数订单
/// </summary>
[Table("OrderCounting")]
internal class OrderCountingEntity : OrderEntity
{
    /// <summary>
    ///     总数量
    /// </summary>
    [Required]
    [Column("TotalCounts")]
    public required double TotalCounts { get; set; }

    /// <summary>
    ///     初始数量, 开始服务时候已完成的数量
    /// </summary>
    [Required]
    [Column("InitCounts")]
    public required double InitCounts { get; set; }


    /// <summary>
    ///     所有划扣
    /// </summary>
    public ICollection<ConsumeCountingEntity> Consumes { get; } = [];
}