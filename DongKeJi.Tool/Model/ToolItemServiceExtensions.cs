using System.Reflection;
using DongKeJi.Extensions;
using DongKeJi.Module;
using Microsoft.Extensions.DependencyInjection;
using Wpf.Ui.Controls;

namespace DongKeJi.Tool.Model;

/// <summary>
/// 工具元素扩展
/// </summary>
public static class ToolItemServiceExtensions
{
    /// <summary>
    /// 添加模块下的工具
    /// </summary>
    /// <typeparam name="TModule"></typeparam>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddToolItems<TModule>(this IServiceCollection services)
        where TModule : IModule
    {
        var toolItems = typeof(TModule).Assembly.GetToolItems();

        foreach(var i in toolItems)
        {
            var pageInjectDescribe = ServiceDescriptor.Describe(i.PageType, i.PageType, i.LifeTime);
            services.Add(pageInjectDescribe);
        }

        return services.AddSingleton<IModuleTool>(new ModuleTool(TModule.Info, toolItems));
    }

    /// <summary>
    ///     获取目标程序集中 标注<see cref="IToolItem"/> 的菜单元素
    /// </summary>
    /// <param name="assembly"></param>
    /// <returns></returns>
    public static List<ToolItemAttribute> GetToolItems(this Assembly assembly)
    {
        var types = assembly.GetTypes();
        List<ToolItemAttribute> menuItems = [];

        foreach (var type in types)
        {
            if (type.IsAbstract) continue;
            if (type.IsInterface) continue;

            //获取注入信息
            var toolInfo = type.GetCustomAttributes().OfType<ToolItemAttribute>().FirstOrDefault();
            if (toolInfo is null) continue;

            try
            {
                ToolItemAttribute.ValidatePageType(type);

                toolInfo.PageType = type;
                menuItems.Add(toolInfo);
            }
            catch
            {
                //TODO: 验证失败先跳过
            }
        }

        return menuItems;
    }
}