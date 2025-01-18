using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace DongKeJi.Deploy.Service;


public interface IVersionService
{
    Version GetLocalLauncherVersion();

    Version GetLauncherVersion(string launcherPath);
}

internal class VersionService(IConfig config) : IVersionService
{
    public Version GetLocalLauncherVersion()
    {
        var launcherPath = Path.Combine(AppContext.BaseDirectory, config.LauncherFileName);
        return GetLauncherVersion(launcherPath);
    }

    public Version GetLauncherVersion(string launcherPath)
    {
        var versionString = FileVersionInfo.GetVersionInfo(launcherPath).ProductVersion;
        ArgumentException.ThrowIfNullOrWhiteSpace(versionString);

        var match = Regex.Match(versionString, @"^\d+(\.\d+){1,3}");
        if (!match.Success)
        {
            throw new FormatException("版本字符串格式无效");
        }

        return new Version(match.Value);
    }
}