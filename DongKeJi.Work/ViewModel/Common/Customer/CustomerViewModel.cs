using CommunityToolkit.Mvvm.ComponentModel;
using DongKeJi.Common;
using DongKeJi.Common.ViewModel;
using System.ComponentModel.DataAnnotations;

namespace DongKeJi.Work.ViewModel.Common.Customer;

/// <summary>
///     机构
/// </summary>
public partial class CustomerViewModel : DataViewModel, IEmptyable<CustomerViewModel>
{
    /// <summary>
    ///     区域
    /// </summary>
    [ObservableProperty] private string? _area;

    /// <summary>
    ///     名称
    /// </summary>
    [ObservableProperty] 
    [Required(ErrorMessage = "机构名称不可为空")]
    [MinLength(6, ErrorMessage = "机构名称长度不可小于1")]
    [MaxLength(32, ErrorMessage = "机构名称长度不可大于32")]
    private string _name = string.Empty;


    public bool IsEmpty => this == Empty;

    public static CustomerViewModel Empty { get; } = new() { Id = Guid.Empty };
}