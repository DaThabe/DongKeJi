using Microsoft.Extensions.Hosting;

namespace DongKeJi.Module;

public static class ModuleExtensions
{
    /// <summary>
    ///     所有已加载模块
    /// </summary>
    private static Dictionary<Type, IModuleInfo> Modules { get; } = new();

    /// <summary>
    /// 已加载的模块元信息
    /// </summary>
    public static IEnumerable<IModuleInfo> MetaInfos => Modules.Select(x => x.Value);

    /// <summary>
    ///     注册模块信息
    /// </summary>
    /// <typeparam name="TModule"></typeparam>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IHostBuilder RegisterModule<TModule>(this IHostBuilder builder)
        where TModule : IModule
    {
        var type = typeof(TModule);
        if (!Modules.TryGetValue(type, out var moduleMetaInfo))
        {
            TModule.Configure(builder);
            Modules[type] = TModule.Info;
        }

        return builder;
    }
}