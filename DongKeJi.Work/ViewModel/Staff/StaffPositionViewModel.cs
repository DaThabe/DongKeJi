using CommunityToolkit.Mvvm.ComponentModel;
using DongKeJi.ViewModel;
using DongKeJi.Work.Model.Entity.Staff;
using System.ComponentModel.DataAnnotations;

namespace DongKeJi.Work.ViewModel.Staff;

/// <summary>
///     员工职位
/// </summary>
public partial class StaffPositionViewModel : EntityViewModel
{
    /// <summary>
    ///     标题
    /// </summary>
    [ObservableProperty]
    [Required(ErrorMessage = "职位标题不可为空")]
    [RegularExpression(@"^[^\s\\][^\x5C]*$", ErrorMessage = "职位标题首字符不可为空且不可使用转义字符")]
    [MinLength(1, ErrorMessage = "职位标题长度不可小于1")]
    [MaxLength(128, ErrorMessage = "职位标题长度不可大于128")] 
    private string _title = string.Empty;

    /// <summary>
    ///     描述
    /// </summary>
    [ObservableProperty] private string? _describe;


    /// <summary>
    ///     类型
    /// </summary>
    [ObservableProperty] private StaffPositionType _type = StaffPositionType.None;


    partial void OnTitleChanging(string value) => ValidateProperty(value, nameof(Title));
}