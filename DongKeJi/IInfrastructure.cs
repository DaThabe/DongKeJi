namespace DongKeJi;

/// <summary>
///     基础建设
/// </summary>
public interface IInfrastructure
{
    /// <summary>
    ///     所有已注册业务
    /// </summary>
    IServiceProvider ServiceProvider { get; }
}