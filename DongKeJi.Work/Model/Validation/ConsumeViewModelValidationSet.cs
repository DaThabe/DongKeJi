using DongKeJi.Common.Inject;
using DongKeJi.Common.Validation.Rule;
using DongKeJi.Common.Validation;
using DongKeJi.Work.ViewModel.Common.Consume;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;
using DongKeJi.Common.Validation.Set;

namespace DongKeJi.Work.Model.Validation;


[Inject(ServiceLifetime.Singleton)]
[Inject(ServiceLifetime.Singleton, typeof(IValidation<ConsumeViewModel>))]
public class ConsumeViewModelValidationSet : ValidationSet<ConsumeViewModel>
{
    /// <summary>
    /// 名称验证
    /// </summary>
    public NameValidationSet Name { get; }


    public ConsumeViewModelValidationSet()
    {
        Name = NameValidationSet.Default;


        Add(x => x.Title, Name);
        Add(new LambdaValidationRule<ConsumeViewModel>(ConsumeValidate));
    }

    private ValidationResult ConsumeValidate(ConsumeViewModel consumeVm)
    {
        return consumeVm switch
        {
            ConsumeTimingViewModel { ConsumeDays: < 0 } =>
                new ValidationResult(false, "划扣天数不可小于0"),
            ConsumeTimingViewModel { ConsumeDays: > 99999 } =>
                new ValidationResult(false, "划扣天数不可大于99999"),
            ConsumeCountingViewModel { ConsumeCounts: < 0 } =>
                new ValidationResult(false, "划扣张数不可小于0"),
            ConsumeCountingViewModel { ConsumeCounts: > 99999 } =>
                new ValidationResult(false, "划扣张数不可大于99999"),
            ConsumeMixingViewModel { ConsumeDays: < 0 } =>
                new ValidationResult(false, "划扣天数不可小于0"),
            ConsumeMixingViewModel { ConsumeDays: > 99999 } =>
                new ValidationResult(false, "划扣天数不可大于99999"),
            ConsumeMixingViewModel { ConsumeCounts: < 0 } =>
                new ValidationResult(false, "划扣张数不可小于0"),
            ConsumeMixingViewModel { ConsumeCounts: > 99999 } =>
                new ValidationResult(false, "划扣张数不可大于99999"),
            _ => ValidationResult.ValidResult
        };
    }

    public override string ToString()
    {
        return "划扣验证";
    }
}

