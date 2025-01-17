using Newtonsoft.Json;

namespace DongKeJi.Deploy.Model;


/// <summary>
/// 版本信息
/// </summary>
public class VersionInfo
{
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
    /// 是否强制更新
    /// </summary>
    [JsonProperty(nameof(IsMandatory))]
    public bool IsMandatory { get; set; }

    /// <summary>
    /// 更新类型
    /// </summary>
    [JsonProperty(nameof(UpdateType))]
    public UpdateTypeEnum UpdateType { get; set; } = UpdateTypeEnum.Patch;

    /// <summary>
    /// 更新大小（单位：MB）
    /// </summary>
    [JsonProperty(nameof(UpdateSize))]
    public double UpdateSize { get; set; }

    /// <summary>
    /// 文件
    /// </summary>
    [JsonProperty(nameof(Files))]
    public VersionFileInfo[] Files { get; set; } = [];
}


public static class VersionInfoExtensions
{

    public static string ToJson(this VersionInfo versionInfo)
    {
        return JsonConvert.SerializeObject(versionInfo);
    }

    public static VersionInfo CreateByJson(string json)
    {
        var info = JsonConvert.DeserializeObject<VersionInfo>(json);
        ArgumentNullException.ThrowIfNull(info);

        return info;
    }
}