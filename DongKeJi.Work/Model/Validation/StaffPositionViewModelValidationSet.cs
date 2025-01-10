using DongKeJi.Common.Inject;
using DongKeJi.Common.Validation;
using DongKeJi.Common.Validation.Rule;
using DongKeJi.Work.Model.Entity.Staff;
using DongKeJi.Work.ViewModel.Common.Staff;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;
using DongKeJi.Common.Validation.Set;

namespace DongKeJi.Work.Model.Validation;

/// <summary>
/// 职位数据验证
/// </summary>
[Inject(ServiceLifetime.Singleton)]
[Inject(ServiceLifetime.Singleton, typeof(IValidation<StaffPositionViewModel>))]
public class StaffPositionViewModelValidationSet : ValidationSet<StaffPositionViewModel>
{
    /// <summary>
    /// 默认值
    /// </summary>
    public static StaffPositionViewModelValidationSet Default { get; } = [];


    /// <summary>
    /// 名称验证
    /// </summary>
    public NameValidationSet Title { get; init; } = NameValidationSet.Default;

    /// <summary>
    /// 职位验证
    /// </summary>
    public ValidationRule<StaffPositionType> Type { get; init; } = new LambdaValidationRule<StaffPositionType>(x =>
    {
        if (x == StaffPositionType.None)
        {
            return new ValidationResult(false, "职位类型不明确");
        }

        return ValidationResult.ValidResult; ;
    });


    public StaffPositionViewModelValidationSet()
    {
        Add(x => x.Title, Title);
        Add(x => x.Type, Type);
    }

    public override string ToString()
    {
        return "员工职位验证";
    }
}