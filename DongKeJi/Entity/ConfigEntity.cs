using System.ComponentModel.DataAnnotations.Schema;


namespace DongKeJi.Entity;

/// <summary>
///     配置
/// </summary>
[Table("Config")]
public class ConfigEntity : EntityBase
{
    /// <summary>
    ///     名称
    /// </summary>
    [Column("Key")]
    public required string Key { get; set; }

    /// <summary>
    ///     名称
    /// </summary>
    [Column("Json")]
    public required string JsonStringValue { get; set; }
}