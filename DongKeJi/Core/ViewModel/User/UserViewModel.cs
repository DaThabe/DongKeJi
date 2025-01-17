using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using DongKeJi.Inject;
using DongKeJi.ViewModel;
using Microsoft.Extensions.DependencyInjection;

namespace DongKeJi.Core.ViewModel.User;


[Inject(ServiceLifetime.Transient)]
public partial class UserViewModel : EntityViewModel
{
    /// <summary>
    ///     用户名
    /// </summary>
    [ObservableProperty]
    [MinLength(1, ErrorMessage = "名称长度不可小于1")]
    [MaxLength(32, ErrorMessage = "名称长度不可大于32")]
    private string _name = string.Empty;

    
    public override string ToString()
    {
        return $"用户: {Name}";
    }


    partial void OnNameChanging(string value) => ValidateProperty(value, nameof(ViewModel.User.UserViewModel.Name));
}