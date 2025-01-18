using Newtonsoft.Json;

namespace DongKeJi.Deploy.Model;

/// <summary>
/// 文件信息
/// </summary>
public class VersionFileItem
{
    /// <summary>
    /// 下载地址
    /// </summary>
    [JsonProperty(nameof(DownloadUrl))]
    public string DownloadUrl { get; set; } = "";

    /// <summary>
    /// 文件名称
    /// </summary>
    [JsonProperty(nameof(Name))]
    public string Name { get; set; } = "";

    /// <summary>
    /// 文件校验码
    /// </summary>
    [JsonProperty(nameof(Md5))]
    public string Md5 { get; set; } = "";

    /// <summary>
    /// 文件大小
    /// </summary>
    [JsonProperty(nameof(Length))]
    public long Length { get; set; }
}