using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using DongKeJi.ViewModel;

namespace DongKeJi.Work.ViewModel.Consume;

/// <summary>
///     划扣
/// </summary>
public abstract partial class ConsumeViewModel : EntityViewModel, IWorkEntityViewModel
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



    partial void OnCreateTimeChanged(DateTime value) => ValidateProperty(value, nameof(CreateTime));

    partial void OnTitleChanged(string value) => ValidateProperty(value, nameof(Title));
}