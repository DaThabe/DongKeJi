using System.Reflection;
using Microsoft.Extensions.Hosting;

namespace DongKeJi.Module;

public static class ModuleExtensions
{
    /// <summary>
    ///     所有已加载模块
    /// </summary>
    private static Dictionary<AssemblyName, IModule> Modules { get; } = new();

    /// <summary>
    /// 已加载的模块元信息
    /// </summary>
    public static IEnumerable<IModuleMetaInfo> MetaInfos => Modules.Select(x => x.Value.MetaInfo);



    /// <summary>
    ///     注册模块信息
    /// </summary>
    /// <typeparam name="TModule"></typeparam>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IHostBuilder RegisterModule<TModule>(this IHostBuilder builder)
        where TModule : IModule, new()
    {
        var assemblyName = typeof(TModule).Assembly.GetName();

        if (Modules.TryGetValue(assemblyName, out _)) return builder;

        TModule module = new();
        module.Configure(builder);

        Modules[assemblyName] = module;
        return builder;
    }

    /// <summary>
    ///     获取模块
    /// </summary>
    /// <typeparam name="TModule"></typeparam>
    /// <param name="context"></param>
    /// <returns></returns>
    public static TModule? GetModule<TModule>(this IModuleContext context)
        where TModule : class, IModule
    {
        var assemblyName = context.GetType().Assembly.GetName();
        Modules.TryGetValue(assemblyName, out var module);

        return module as TModule;
    }
}