using DongKeJi.Common.Inject;
using DongKeJi.Common.Validation;
using DongKeJi.Common.Validation.Rule;
using DongKeJi.Common.Validation.Set;
using DongKeJi.Work.ViewModel.Common.Order;
using Microsoft.Extensions.DependencyInjection;

namespace DongKeJi.Work.Model.Validation;

/// <summary>
/// 订单数据验证
/// </summary>
[Inject(ServiceLifetime.Singleton)]
[Inject(ServiceLifetime.Singleton, typeof(IValidation<OrderViewModel>))]
public class OrderViewModelValidationSet : ValidationSet<OrderViewModel>
{
    /// <summary>
    /// 名称验证
    /// </summary>
    public NameValidationSet Name { get; }

    /// <summary>
    /// 价格验证
    /// </summary>
    public RangeValidationRule<double> Price { get; }



    public OrderViewModelValidationSet()
    {
        Name = NameValidationSet.Default;
        Price = new RangeValidationRule<double>
        {
            Minimum = 1,
            Maximum = 99999,
            MinimumMessage = "订单价格不能低于1",
            MaximumMessage = "订单价格不能高于99999"
        };

        Add(x => x.Name, Name);
        Add(x => x.Price, Price);
    }

    public override string ToString()
    {
        return "订单验证";
    }
}