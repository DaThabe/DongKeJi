using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using DongKeJi.ViewModel;

namespace DongKeJi.Work.ViewModel.Customer;

/// <summary>
///     机构
/// </summary>
public partial class CustomerViewModel : EntityViewModel, IWorkEntityViewModel
{
    /// <summary>
    ///     名称
    /// </summary>
    [ObservableProperty]
    [Required(ErrorMessage = "机构名称不可为空")]
    [MinLength(6, ErrorMessage = "机构名称长度不可小于1")]
    [MaxLength(32, ErrorMessage = "机构名称长度不可大于32")]
    private string _name = string.Empty;

    /// <summary>
    ///     区域
    /// </summary>
    [ObservableProperty] 
    private string? _area;
}