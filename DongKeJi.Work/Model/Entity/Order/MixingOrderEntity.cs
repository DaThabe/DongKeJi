using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DongKeJi.Work.Model.Entity.Consume;

namespace DongKeJi.Work.Model.Entity.Order;

/// <summary>
///     混合订单
/// </summary>
[Table("OrderMixing")]
internal class MixingOrderEntity : OrderEntity
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
    ///     总天数
    /// </summary>
    [Required]
    [Column("TotalDays")]
    public required double TotalDays { get; set; }


    /// <summary>
    ///     初始天数, 开始服务时候已完成的
    /// </summary>
    [Required]
    [Column("InitDays")]
    public required double InitDays { get; set; }


    /// <summary>
    ///     所有划扣
    /// </summary>
    public ICollection<MixingConsumeEntity> Consumes { get; } = [];
}