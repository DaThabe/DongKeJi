using System.Text;
using DongKeJi.Extensions;
using DongKeJi.Inject;
using DongKeJi.Work.Model;
using DongKeJi.Work.Model.Entity.Order;
using DongKeJi.Work.ViewModel.Consume;
using DongKeJi.Work.ViewModel.Customer;
using DongKeJi.Work.ViewModel.Order;
using Microsoft.Extensions.DependencyInjection;

namespace DongKeJi.Work.Service;

/// <summary>
/// 工资提成服务
/// </summary>
public interface IWagesService
{
    /// <summary>
    /// 导出月消耗明细
    /// </summary>
    /// <param name="designer"></param>
    /// <param name="date"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    ValueTask<string> ExportMonthlyConsumeAsync(
        IIdentifiable designer, 
        DateTime date,
        CancellationToken cancellation = default);

}


[Inject(ServiceLifetime.Singleton, typeof(IWagesService))]
internal class WagesService(
    IConsumeService consumeService,
    IOrderService orderService
    ) : IWagesService
{
    public async ValueTask<string> ExportMonthlyConsumeAsync(IIdentifiable designer, DateTime date, CancellationToken cancellation = default)
    {
        Dictionary<OrderType, StringBuilder> infoBuilders = new()
        {
            { OrderType.Timing, new StringBuilder() },
            { OrderType.Mixing, new StringBuilder() },
            { OrderType.Counting, new StringBuilder() }
        };

        var orders = await orderService.GetAllByStaffAsync(designer, cancellation: cancellation);

        foreach (var order in orders)
        {
            var consumes = (await consumeService.GetAllByOrderAsync(order, cancellation: cancellation))
                .ToList();

            var orderType = consumes.FirstOrDefault()?.GetOrderType() ?? OrderType.Unknown;
            if(orderType == OrderType.Unknown) continue;
            
            var consumeString = FormatConsume(orderType, consumes);
            if (string.IsNullOrWhiteSpace(consumeString)) continue;

            var customer = await orderService.GetCustomerAsync(order, cancellation);
            var customerString =  FormatCustomer(customer);


            infoBuilders[orderType]
                .AppendLine()
                .Append(customerString).Append('-').Append(consumeString).AppendLine()
                .Append('￥').Append(order.Price).AppendLine();
        }

        StringBuilder infoBuilder = new();

        TryAddTiming(ref infoBuilder);
        TryAddCounting(ref infoBuilder);
        TryAddMixing(ref infoBuilder);

        return infoBuilder.ToString();


        void TryAddTiming(ref StringBuilder sb)
        {
            var value = infoBuilders[OrderType.Timing].ToString();
            if (string.IsNullOrWhiteSpace(value)) return;

            sb.AppendLine("-------------包月-------------");
            sb.AppendLine(value);
        }

        void TryAddCounting(ref StringBuilder sb)
        {
            var value = infoBuilders[OrderType.Counting].ToString();
            if (string.IsNullOrWhiteSpace(value)) return;

            infoBuilder.AppendLine();
            sb.AppendLine("-----------包张/包天-----------");
            sb.AppendLine(value);
        }

        void TryAddMixing(ref StringBuilder sb)
        {
            var value = infoBuilders[OrderType.Mixing].ToString();
            if (string.IsNullOrWhiteSpace(value)) return;

            infoBuilder.AppendLine();
            sb.AppendLine("-------------散单-------------");
            sb.AppendLine(value);
        }


        string? FormatConsume(OrderType orderType, List<ConsumeViewModel> consumes)
        {
            if (orderType == OrderType.Timing)
            {
                var value = consumes.OfType<ConsumeTimingViewModel>().Sum(x => x.ConsumeDays);
                
                return $"{value} 天";
            }

            if (orderType == OrderType.Counting)
            {
                var value = consumes.OfType<ConsumeCountingViewModel>().Sum(x => x.ConsumeCounts);
                
                return $"{value} 张";
            }

            if (orderType == OrderType.Mixing)
            {
                var consumeTimings = consumes.OfType<ConsumeMixingViewModel>().ToArray();
                var day = consumeTimings.Sum(x => x.ConsumeDays);
                var count = consumeTimings.Sum(x => x.ConsumeCounts);

                return $"{day} 天 {count} 张";
            }

            return null;
        }

        string FormatCustomer(CustomerViewModel customer)
        {
            return customer.Name;
        }
    }
}


/*

-------------包月-------------

机构名称, 29 天
￥3999

-----------包张/包天-----------

机构名称, 5 天 20 张
￥3999

-------------散单-------------

机构名称, 5 张
￥1000

 * 
 */