using DongKeJi.Module;
using Microsoft.Extensions.Hosting;

namespace DongKeJi;

/// <summary>
/// 内核模块
/// </summary>
public class BaseModule : IModule
{
    private static readonly ModuleMetaInfo ModuleMetaInfo = new()
    {
        Id = Guid.NewGuid(),
        Version = new Version(0, 0, 1),
        Title = "内核",
        Developers = [ "DaThabe" ],
        Describe = """
                   内核模块
                   -程序的底层代码
                   """,
        CreatedAt = new DateTime(2024, 6, 16),
        ReleaseDate = new DateTime(2025, 12, 24),
        Dependencies = [ ]
    };

    public IModuleMetaInfo MetaInfo => ModuleMetaInfo;

    public void Configure(IHostBuilder builder)
    {
    }
}