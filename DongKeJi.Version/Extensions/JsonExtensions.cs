using Newtonsoft.Json;

namespace DongKeJi.Version.Extensions;

public static class JsonExtensions
{
    /// <summary>
    /// 转为Json
    /// </summary>
    /// <param name="versionInfo"></param>
    /// <returns></returns>
    public static string ToJson(this object versionInfo)
    {
        return JsonConvert.SerializeObject(versionInfo);
    }

    /// <summary>
    /// 将Json转为对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="json"></param>
    /// <returns></returns>
    public static T? ToObject<T>(this string json)
    {
        var info = JsonConvert.DeserializeObject<T>(json);
        return info;
    }
}