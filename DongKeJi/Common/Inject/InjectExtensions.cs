using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace DongKeJi.Common.Inject;

/// <summary>
///     注入
/// </summary>
public static class InjectExtensions
{
    /// <summary>
    ///     添加指定类型所在程序集的所有自动注入内容
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddAutoInject<T>(this IServiceCollection services)
    {
        var injectDescriptors = typeof(T).Assembly.GetInjectDescriptors();

        foreach (var i in injectDescriptors) services.Add(i);

        return services;
    }

    /// <summary>
    ///     获取指定程序集的所有注入信息
    /// </summary>
    /// <param name="assembly"></param>
    /// <returns></returns>
    public static IEnumerable<ServiceDescriptor> GetInjectDescriptors(this Assembly assembly)
    {
        var types = assembly.GetTypes();
        List<IInjectDescriptor> injectDescriptors = [];

        foreach (var type in types)
        {
            if (type.IsAbstract) continue;
            if (type.IsInterface) continue;

            //获取注入信息
            var injectAttribute = type.GetCustomAttributes().OfType<IInjectDescriptor>().FirstOrDefault();
            if (injectAttribute is null) continue;

            //设置具体类型
            injectAttribute.ImplementationType = type;

            ////如果没有设置业务类型, 但是有一个接口, 那就设置为业务类型
            //var serviceTypes = type.GetInterfaces();
            //if (injectAttribute.ServiceType is null && serviceTypes.Length == 1)
            //{
            //    injectAttribute.ServiceType = serviceTypes.FirstOrDefault();
            //}

            injectDescriptors.Add(injectAttribute);
        }

        return injectDescriptors.Select(ToServiceDescriptor);
    }

    /// <summary>
    ///     转为<see cref="ServiceDescriptor" />
    /// </summary>
    /// <param name="inject"></param>
    /// <returns></returns>
    private static ServiceDescriptor ToServiceDescriptor(this IInjectDescriptor inject)
    {
        //无业务类型
        if (inject.ServiceType is null)
        {
            if (inject.ServiceKey is null)
                return new ServiceDescriptor(inject.ImplementationType, inject.ImplementationType,
                    inject.ServiceLifetime);

            return new ServiceDescriptor(inject.ImplementationType, inject.ServiceKey, inject.ImplementationType,
                inject.ServiceLifetime);
        }

        //有业务类型
        if (inject.ServiceKey is null)
            return new ServiceDescriptor(inject.ServiceType, inject.ImplementationType, inject.ServiceLifetime);

        return new ServiceDescriptor(inject.ServiceType, inject.ServiceKey, inject.ImplementationType,
            inject.ServiceLifetime);
    }
}