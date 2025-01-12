using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DongKeJi.Validation;
using System.ComponentModel;

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

    public IEnumerable<ValidationResult> Errors { get; }


    public EntityViewModel()
    {
        Errors = new ErrorObservableCollection(this);
    }

    public void Validate()
    {
        ValidateAllProperties();
    }

    public override string ToString()
    {
        //EntityViewModel - xxxx-xxxx-xxxx
        return $"{GetType().Name} - {Id:N}";
    }
}


file class ErrorObservableCollection : ObservableCollection<ValidationResult>
{
    private readonly ObservableValidator _viewModel;

    public ErrorObservableCollection(ObservableValidator viewModel)
    {
        _viewModel = viewModel;
        viewModel.ErrorsChanged += ViewModel_ErrorsChanged;
    }

    private void ViewModel_ErrorsChanged(object? sender, DataErrorsChangedEventArgs e)
    {
        var errors = _viewModel.GetErrors().ToArray();

        //添加新元素
        foreach (var item in errors.ToArray())
        {
            if (Contains(item)) continue;
            Add(item);
        }

        //删除过时元素
        var itemsToRemove = this.Except(errors).ToList();
        foreach (var item in itemsToRemove)
        {
            Remove(item);
        }
    }
}