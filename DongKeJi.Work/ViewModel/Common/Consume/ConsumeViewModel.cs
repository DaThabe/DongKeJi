using CommunityToolkit.Mvvm.ComponentModel;
using DongKeJi.Common;
using DongKeJi.Common.ViewModel;
using System.ComponentModel.DataAnnotations;

namespace DongKeJi.Work.ViewModel.Common.Consume;

/// <summary>
///     划扣
/// </summary>
public abstract partial class ConsumeViewModel : DataViewModel, IEmptyable<ConsumeViewModel>
{
    /// <summary>
    ///     时间
    /// </summary>
    [ObservableProperty]
    [Required(ErrorMessage = "创建日期不可为空")]
    private DateTime _createTime = DateTime.Now;

    /// <summary>
    ///     标题
    /// </summary>
    [ObservableProperty]
    [Required(ErrorMessage = "划扣标题不可为空")]
    [MinLength(1, ErrorMessage = "划扣标题长度不可小于1")]
    [MaxLength(32, ErrorMessage = "划扣标题长度不可大于32")]
    private string _title = "日常划扣";


    public bool IsEmpty => this == Empty;
    public static ConsumeViewModel Empty => EmptyConsumeViewModel.Instance;
}

file class EmptyConsumeViewModel : ConsumeViewModel
{
    private EmptyConsumeViewModel()
    {
        Title = string.Empty;
        CreateTime = DateTime.MinValue;
    }

    public static EmptyConsumeViewModel Instance { get; } = new();
}