using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using DongKeJi.ViewModel;

namespace DongKeJi.Work.ViewModel.Customer;

/// <summary>
///     机构
/// </summary>
public partial class CustomerViewModel : EntityViewModel
{
    /// <summary>
    ///     名称
    /// </summary>
    [ObservableProperty]
    [Required(ErrorMessage = "机构名称不可为空")]
    [RegularExpression(@"^[^\s\\][^\x5C]*$", ErrorMessage = "机构名称首字符不可为空且不可使用转义字符")]
    [MinLength(1, ErrorMessage = "机构名称长度不可小于1")]
    [MaxLength(128, ErrorMessage = "机构名称长度不可大于128")]
    private string _name = string.Empty;

    /// <summary>
    ///     区域
    /// </summary>
    [ObservableProperty] 
    private string? _area;


    partial void OnNameChanging(string value) => ValidateProperty(value, nameof(Name));


    public override string ToString()
    {
        return $"{Area}-{Name}";
    }
}