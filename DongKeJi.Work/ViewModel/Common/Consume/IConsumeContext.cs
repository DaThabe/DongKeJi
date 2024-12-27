namespace DongKeJi.Work.ViewModel.Common.Consume;

/// <summary>
///     订单上下文
/// </summary>
public interface IConsumeContext
{
    /// <summary>
    ///     当前划扣
    /// </summary>
    ConsumeViewModel Consume { get; set; }
}