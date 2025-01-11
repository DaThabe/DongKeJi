using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DongKeJi.Entity;

namespace DongKeJi.Work.Model.Entity.Staff;

/// <summary>
///     员工职位
/// </summary>
[Table("Position")]
internal class StaffPositionEntity : EntityBase
{
    /// <summary>
    ///     类型
    /// </summary>
    [Column("Type")]
    public required StaffPositionType Type { get; set; }

    /// <summary>
    ///     标题
    /// </summary>
    [Column("Title")]
    [MinLength(1)]
    [MaxLength(32)]
    public required string Title { get; set; }

    /// <summary>
    ///     描述
    /// </summary>
    [Column("Describe")]
    [MinLength(0)]
    [MaxLength(256)]
    public string? Describe { get; set; }


    public virtual ICollection<StaffEntity> Staffs { get; init; } = [];
}