using CommunityToolkit.Mvvm.ComponentModel;
using DongKeJi.Common;
using DongKeJi.Common.ViewModel;

namespace DongKeJi.Work.ViewModel.Common.Consume;

/// <summary>
///     划扣
/// </summary>
public abstract partial class ConsumeViewModel : IdentifiableViewModel, IEmptyable<ConsumeViewModel>
{
    /// <summary>
    ///     时间
    /// </summary>
    [ObservableProperty] private DateTime _createTime = DateTime.Now;

    /// <summary>
    ///     标题
    /// </summary>
    [ObservableProperty] private string _title = "日常划扣";


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