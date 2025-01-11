using DongKeJi.Module;
using Microsoft.Extensions.Hosting;

namespace DongKeJi;

/// <summary>
/// �ں�ģ��
/// </summary>
public class BaseModule : IModule
{
    private static readonly ModuleMetaInfo ModuleMetaInfo = new()
    {
        Id = Guid.NewGuid(),
        Version = new Version(0, 0, 1),
        Title = "�ں�",
        Developers = [ "DaThabe" ],
        Describe = """
                   �ں�ģ��
                   -����ĵײ����
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