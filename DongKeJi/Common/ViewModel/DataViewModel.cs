using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DongKeJi.Common.Validation;

namespace DongKeJi.Common.ViewModel;

/// <summary>
///     数据视图模型
/// </summary>
public class DataViewModel : ObservableValidator, IViewModel, IIdentifiable, IValidation
{
    /// <summary>
    ///     id
    /// </summary>
    public Guid Id { get; init; } = Guid.NewGuid();

    
    public IEnumerable<ValidationResult> Validate()
    {
        ClearErrors();

        //TODO: 是否能获取到继承类的所有属性呢?
        ValidateAllProperties();
        return GetErrors();
    }

    public override string ToString()
    {
        return $"数据Id: {Id:N}";
    }
}