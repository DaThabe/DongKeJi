using Newtonsoft.Json;

namespace DongKeJi.Core.Model.Update;


/// <summary>
/// 版本元素
/// </summary>
public class VersionItem
{
    /// <summary>
    /// 是否是预览版本
    /// </summary>
    [JsonProperty(nameof(IsPreview))]
    public bool IsPreview { get; set; }

    /// <summary>
    /// 当前版本
    /// </summary>
    [JsonProperty(nameof(Version))]
    public Version Version { get; set; } = new(0, 0, 0);

    /// <summary>
    /// 介绍
    /// </summary>
    [JsonProperty(nameof(Intro))]
    public string Intro { get; set; } = "";

    /// <summary>
    /// 发布时间
    /// </summary>
    [JsonProperty(nameof(ReleaseDate))]
    public DateTime ReleaseDate { get; set; } = DateTime.Now;

    /// <summary>
    /// 更新大小（单位：MB）
    /// </summary>
    [JsonProperty(nameof(UpdateSize))]
    public double UpdateSize { get; set; }

    /// <summary>
    /// 文件
    /// </summary>
    [JsonProperty(nameof(Files))]
    public VersionFileItem[] Files { get; set; } = [];
}