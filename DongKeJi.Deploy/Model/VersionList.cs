using Newtonsoft.Json;

namespace DongKeJi.Deploy.Model;

/// <summary>
/// 版本列表
/// </summary>
public class VersionList
{
    /// <summary>
    /// 最新版
    /// </summary>
    [JsonProperty(nameof(LatestVersion))]
    public Version? LatestVersion { get; set; }

    /// <summary>
    /// 所有版本
    /// </summary>
    [JsonProperty(nameof(Versions))]
    public List<VersionItem> Versions { get; set; } = [];
}