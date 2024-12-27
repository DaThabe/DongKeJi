using CommunityToolkit.Mvvm.ComponentModel;
using DongKeJi.Common.Inject;
using DongKeJi.Common.ViewModel;
using Microsoft.Extensions.DependencyInjection;

namespace DongKeJi.ViewModel.User;

[Inject(ServiceLifetime.Transient)]
public partial class UserViewModel : IdentifiableViewModel
{
    /// <summary>
    ///     用户名
    /// </summary>
    [ObservableProperty] private string _name = string.Empty;

    public static UserViewModel Empty { get; } = new()
    {
        Id = Guid.Empty
    };


    public override string ToString()
    {
        return $"用户: {Name}";
    }
}