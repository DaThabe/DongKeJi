using System.ComponentModel.DataAnnotations.Schema;
using DongKeJi.Entity;

namespace DongKeJi.Core.Model.Entity;

/// <summary>
///     用户
/// </summary>
[Table("User")]
internal class UserEntity : EntityBase
{
    /// <summary>
    ///     名称
    /// </summary>
    [Column("Name")]
    public required string Name { get; set; }
}