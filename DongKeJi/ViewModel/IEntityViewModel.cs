using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DongKeJi.Validation;
using DongKeJi.Extensions;

namespace DongKeJi.ViewModel;


/// <summary>
/// 实体视图模型
/// </summary>
public interface IEntityViewModel : IViewModel, IIdentifiable, IValidation;


/// <summary>
///     实体视图模型
/// </summary>
public class EntityViewModel : ObservableValidator, IEntityViewModel
{
    /// <summary>
    ///     id
    /// </summary>
    public Guid Id { get; init; } = Guid.NewGuid();

    /// <summary>
    /// 验证错误信息
    /// </summary>
    public ObservableCollection<ValidationResult> Errors { get; }


    public EntityViewModel()
    {
        Errors = new DataViewModelObservableCollection<EntityViewModel>(this);
    }


    public IEnumerable<ValidationResult> Validate()
    {
        ClearErrors();
        ValidateAllProperties();
        return GetErrors();
    }

    public override string ToString()
    {
        return $"数据Id: {Id:N}";
    }
}

file class DataViewModelObservableCollection<TViewModel> : ObservableCollection<ValidationResult>
    where TViewModel : IViewModel, IValidation
{
    public DataViewModelObservableCollection(TViewModel viewModel)
    {
        viewModel.PropertyChanging += (_, e) =>
        {
            var errors = viewModel.Validate().ToArray();

            foreach (var i in errors)
            {
                this.Add(i, x => x != i);
            }

            if (errors.Length > 0)
            {
                throw new ValidationException(errors.First(), null, viewModel);
            }
        };
    }
}