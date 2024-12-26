using Microsoft.Extensions.DependencyInjection;

namespace DongKeJi.Common.Inject;


/// <summary>
/// 标注为注入类型
/// </summary>
/// <param name="serviceLifetime"></param>
[AttributeUsage(AttributeTargets.Class)]
public class InjectAttribute(ServiceLifetime serviceLifetime, Type? serviceType, object? serviceKey) : Attribute, IInjectDescriptor
{
    /// <summary>
    /// 如果为空 最后会替换为反射调用时实际类型
    /// </summary>
    public Type? ServiceType { get; set; } = serviceType;

    /// <summary>
    /// 无需手动赋值, 在反射调用时会填充为实际使用类型
    /// </summary>
    public Type ImplementationType { get; set; } = null!;

    /// <summary>
    /// 键
    /// </summary>
    public object? ServiceKey { get; set; } = serviceKey;

    /// <summary>
    /// 生命周期类型
    /// </summary>
    public ServiceLifetime ServiceLifetime { get; set; } = serviceLifetime;


    public InjectAttribute(ServiceLifetime serviceLifetime, Type serviceType) :
        this(serviceLifetime, serviceType, null)
    {

    }

    public InjectAttribute(ServiceLifetime serviceLifetime) :
        this(serviceLifetime, null, null)
    {

    }



    public override string ToString()
    {
        //Type | Imple : IService(key)
        return $"{ImplementationType}>>>>>{ServiceType}";
    }
}