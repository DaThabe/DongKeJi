using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DongKeJi.Common.Entity;
using Microsoft.EntityFrameworkCore;

namespace DongKeJi.Model.Entity;

/// <summary>
///     用户
/// </summary>
[Index(nameof(Name))]
[Table("User")]
public class UserEntity : EntityBase
{
    /// <summary>
    ///     名称
    /// </summary>
    [Column("Name")]
    [Required(ErrorMessage = "名字不可为空")]
    [MinLength(6, ErrorMessage = "名称长度应>=6位")]
    [MaxLength(32, ErrorMessage = "名称长度需<=32位")]
    public required string Name { get; set; }
}