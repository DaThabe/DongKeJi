using CommunityToolkit.Mvvm.ComponentModel;
using DongKeJi.Common.ViewModel;

namespace DongKeJi.Work.ViewModel.Common.Customer;

/// <summary>
///     机构
/// </summary>
public partial class CustomerViewModel : IdentifiableViewModel, IEmptyable<CustomerViewModel>
{
    /// <summary>
    ///     名称
    /// </summary>
    [ObservableProperty] private string _name = string.Empty;


    /// <summary>
    ///     区域
    /// </summary>
    [ObservableProperty] private string? _area;


    public bool IsEmpty => this == Empty;

    public static CustomerViewModel Empty { get; } = new() { Id = Guid.Empty };
}