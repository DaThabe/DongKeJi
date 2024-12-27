using Microsoft.Extensions.DependencyInjection;

namespace DongKeJi.Common.Inject;

/// <summary>
///     注入信息
/// </summary>
public interface IInjectDescriptor
{
    /// <summary>
    ///     服务类型
    /// </summary>
    Type? ServiceType { get; set; }

    /// <summary>
    ///     实现类型
    /// </summary>
    Type ImplementationType { get; set; }

    /// <summary>
    ///     键
    /// </summary>
    object? ServiceKey { get; set; }

    /// <summary>
    ///     生命周期类型
    /// </summary>
    ServiceLifetime ServiceLifetime { get; set; }
}