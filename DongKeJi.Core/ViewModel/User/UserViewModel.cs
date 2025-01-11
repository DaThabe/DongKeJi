using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using DongKeJi.Inject;
using DongKeJi.ViewModel;
using Microsoft.Extensions.DependencyInjection;

namespace DongKeJi.Core.ViewModel.User;


[Inject(ServiceLifetime.Transient)]
public partial class UserViewModel : EntityViewModel, IDefault<UserViewModel>
{
    public bool IsDefault => this == Default;

    public static UserViewModel Default { get; } = new()
    {
        Id = Guid.Empty
    };



    /// <summary>
    ///     用户名
    /// </summary>
    [ObservableProperty]
    [Required(ErrorMessage = "名称不可缺")]
    [MinLength(1, ErrorMessage = "名称长度不可小于1")]
    [MaxLength(32, ErrorMessage = "名称长度不可大于32")]
    private string _name = string.Empty;

    
    public override string ToString()
    {
        return $"用户: {Name}";
    }
}