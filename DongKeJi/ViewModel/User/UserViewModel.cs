using CommunityToolkit.Mvvm.ComponentModel;
using DongKeJi.Common.Inject;
using DongKeJi.Common.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;

namespace DongKeJi.ViewModel.User;

[Inject(ServiceLifetime.Transient)]
public partial class UserViewModel : DataViewModel
{
    /// <summary>
    ///     用户名
    /// </summary>
    [Required(ErrorMessage = "Name is required.")]
    [MinLength(1, ErrorMessage = "名称长度不可小于1")]
    [MaxLength(32, ErrorMessage = "名称长度不可大于32")]
    [ObservableProperty] 
    private string _name = string.Empty;

    public static UserViewModel Empty { get; } = new()
    {
        Id = Guid.Empty
    };


    public override string ToString()
    {
        return $"用户: {Name}";
    }
}