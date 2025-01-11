using DongKeJi.Work.ViewModel.Common.Consume;

namespace DongKeJi.Work.ViewModel.Consume;

/// <summary>
///     订单上下文
/// </summary>
public interface IConsumeContext
{
    /// <summary>
    ///     当前划扣
    /// </summary>
    ConsumeViewModel SelectedConsume { get; set; }
}