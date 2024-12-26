using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DongKeJi.Common.Entity;
using DongKeJi.Work.Model.Entity.Staff;

namespace DongKeJi.Work.Model.Entity.Consume;

/// <summary>
///     机构划扣信息
/// </summary>
[Table("Consume")]
internal abstract class ConsumeEntity : EntityBase
{
    /// <summary>
    ///     记录时间
    /// </summary>
    [Required]
    [Column("CreateTime")]
    public required DateTime CreateTime { get; set; }

    /// <summary>
    ///     备注
    /// </summary>
    [Column("Title")]
    public required string Title { get; set; }


    /// <summary>
    ///     此次划扣员工
    /// </summary>
    [Column("StaffId")]
    public required Guid StaffId { get; set; }

    /// <summary>
    ///     此次划扣员工
    /// </summary>
    [ForeignKey(nameof(StaffId))]
    public StaffEntity Staff { get; set; } = null!;
}