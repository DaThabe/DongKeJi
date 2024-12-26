namespace DongKeJi.ViewModel.User;

/// <summary>
/// 用户上下文
/// </summary>
public interface IUserContext
{
    /// <summary>
    /// 用户
    /// </summary>
    UserViewModel User { get; set; }
}