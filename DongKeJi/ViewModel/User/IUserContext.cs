namespace DongKeJi.ViewModel.User;

/// <summary>
/// 登录用户上下文
/// </summary>
public interface ILoginUserContext
{
    /// <summary>
    ///     登录用户
    /// </summary>
    UserViewModel LoginUser { get; }
}