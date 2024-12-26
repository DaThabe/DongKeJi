using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DongKeJi.Work.Model.Entity.Consume;

namespace DongKeJi.Work.Model.Entity.Order;

/// <summary>
///     计时订单
/// </summary>
[Table("OrderTiming")]
internal class TimingOrderEntity : OrderEntity
{
    /// <summary>
    ///     总天数
    /// </summary>
    [Required]
    [Column("TotalDays")]
    public required double TotalDays { get; set; }

    /// <summary>
    ///     初始天数, 开始服务时候已完成的天数
    /// </summary>
    [Required]
    [Column("InitDays")]
    public required double InitDays { get; set; }


    /// <summary>
    ///     所有划扣
    /// </summary>
    public ICollection<TimingConsumeEntity> Consumes { get; } = [];
}