using System.Reflection;

namespace DongKeJi.Module;


/// <summary>
/// 模块信息
/// </summary>
public interface IModuleMetaInfo
{
    /// <summary>
    /// id
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// 名称
    /// </summary>
    string Title { get; }

    /// <summary>
    /// 描述
    /// </summary>
    string? Describe { get; }

    /// <summary>
    /// 版本
    /// </summary>
    Version Version { get; }

    /// <summary>
    /// 开发者
    /// </summary>
    string[] Developers { get; }

    /// <summary>
    /// 依赖项
    /// </summary>
    AssemblyName[] Dependencies { get; }

    /// <summary>
    /// 创建时间
    /// </summary>
    DateTime CreatedAt { get; }

    /// <summary>
    /// 发布时间
    /// </summary>
    DateTime ReleaseDate { get; }

    /// <summary>
    /// 标签
    /// </summary>
    string[] Tags { get; }
}


public class ModuleMetaInfo : IModuleMetaInfo
{
    public required Guid Id { get; set; }
    public required string Title { get; set; }
    public string? Describe { get; set; }
    public required Version Version { get; set; }
    public required string[] Developers { get; set; }
    public required AssemblyName[] Dependencies { get; set; }
    public required DateTime CreatedAt { get; set; } 
    public required DateTime ReleaseDate { get; set; }
    public string[] Tags { get; set; } = [];
}