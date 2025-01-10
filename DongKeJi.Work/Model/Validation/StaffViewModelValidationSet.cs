using DongKeJi.Common.Inject;
using DongKeJi.Common.Validation;
using DongKeJi.Common.Validation.Set;
using DongKeJi.Work.ViewModel.Common.Staff;
using Microsoft.Extensions.DependencyInjection;

namespace DongKeJi.Work.Model.Validation;

/// <summary>
/// 员工数据验证
/// </summary>
[Inject(ServiceLifetime.Singleton)]
[Inject(ServiceLifetime.Singleton, typeof(IValidation<StaffViewModel>))]
public class StaffViewModelValidationSet : ValidationSet<StaffViewModel>
{
    /// <summary>
    /// 默认值
    /// </summary>
    public static StaffViewModelValidationSet Default { get; } = [];


    /// <summary>
    /// 名称验证
    /// </summary>
    public NameValidationSet Name { get; init; } = NameValidationSet.Default;


    public StaffViewModelValidationSet()
    {
        Add(x => x.Name, Name);
    }

    public override string ToString()
    {
        return "员工验证";
    }
}