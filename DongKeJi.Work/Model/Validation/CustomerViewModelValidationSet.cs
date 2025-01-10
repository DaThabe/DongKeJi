using DongKeJi.Common.Inject;
using DongKeJi.Common.Validation;
using DongKeJi.Common.Validation.Set;
using DongKeJi.Work.ViewModel.Common.Customer;
using Microsoft.Extensions.DependencyInjection;

namespace DongKeJi.Work.Model.Validation;


/// <summary>
/// 机构数据验证
/// </summary>
[Inject(ServiceLifetime.Singleton)]
[Inject(ServiceLifetime.Singleton, typeof(IValidation<CustomerViewModel>))]
public class CustomerViewModelValidationSet : ValidationSet<CustomerViewModel>
{
    public static CustomerViewModelValidationSet Default { get; } = [];


    /// <summary>
    /// 名称验证
    /// </summary>
    public NameValidationSet Name { get; }


    public CustomerViewModelValidationSet()
    {
        Name = NameValidationSet.Default;

        Add(x => x.Name, Name);
    }

    public override string ToString()
    {
        return "机构验证";
    }
}