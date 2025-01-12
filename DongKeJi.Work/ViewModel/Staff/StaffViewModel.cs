using CommunityToolkit.Mvvm.ComponentModel;
using DongKeJi.ViewModel;
using System.ComponentModel.DataAnnotations;

namespace DongKeJi.Work.ViewModel.Staff;

/// <summary>
///     员工
/// </summary>
public partial class StaffViewModel : EntityViewModel
{
    /// <summary>
    ///     用户名
    /// </summary>
    [ObservableProperty]
    [Required(ErrorMessage = "员工名称不可为空")]
    [RegularExpression(@"^[^\s\\][^\x5C]*$", ErrorMessage = "员工名称首字符不可为空且不可使用转义字符")]
    [MinLength(1, ErrorMessage = "员工名称长度不可小于1")]
    [MaxLength(128, ErrorMessage = "员工名称长度不可大于128")]
    private string _name = string.Empty;


    partial void OnNameChanging(string value) => ValidateProperty(value, nameof(Name));
}