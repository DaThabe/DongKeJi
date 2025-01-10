using DongKeJi.Common.Inject;
using DongKeJi.Common.Validation;
using DongKeJi.Common.Validation.Set;
using DongKeJi.ViewModel.User;
using Microsoft.Extensions.DependencyInjection;

namespace DongKeJi.Model.Validation;


/// <summary>
/// 用户数据验证
/// </summary>
[Inject(ServiceLifetime.Singleton)]
[Inject(ServiceLifetime.Singleton, typeof(IValidation<UserViewModel>))]
public class UserViewModelValidationSet : ValidationSet<UserViewModel>
{
    public static UserViewModelValidationSet Default { get; } = [];


    /// <summary>
    /// 名称验证
    /// </summary>
    public NameValidationSet Name { get; init; } = NameValidationSet.Default;


    public UserViewModelValidationSet()
    {
        Add(x => x.Name, Name);
    }
}