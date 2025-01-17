using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using DongKeJi.Validation;
using DongKeJi.ViewModel;
using DongKeJi.Entity;
using DongKeJi.Exceptions;

namespace DongKeJi.Database;


public interface IAutoUpdateBuilder : IDisposable
{
    /// <summary>
    /// 是否暂停更新
    /// </summary>
    bool IsStop { get; set; }
}

public interface IAutoUpdateBuilder<out TViewModel> : IAutoUpdateBuilder
    where TViewModel : IViewModel, IIdentifiable
{
    /// <summary>
    /// 视图模型
    /// </summary>
    TViewModel ViewModel { get; }


    /// <summary>
    /// 更新中
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    IAutoUpdateBuilder<TViewModel> OnUpdating(Action<TViewModel> action);

    /// <summary>
    /// 更新完成
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    IAutoUpdateBuilder<TViewModel> OnUpdated(Action<TViewModel> action);

    /// <summary>
    /// 发生错误
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    IAutoUpdateBuilder<TViewModel> OnFaulted(Action<Exception> action);
}

public partial class AutoUpdateBuilder<TEntity, TViewModel> : ObservableViewModel, IAutoUpdateBuilder<TViewModel>
    where TEntity : EntityBase
    where TViewModel : IEntityViewModel
{
    private readonly DbContext _dbContext;
    private readonly IMapper _mapper;

    /// <summary>
    /// 更新过程委托
    /// </summary>
    private PropertyChangedEventHandler? _propertyChangedEventHandler;

    private CancellationTokenSource _cancellationTokenSource = new();


    /// <summary>
    /// 是否暂停更新
    /// </summary>
    [ObservableProperty]
    private bool _isStop;

    /// <summary>
    /// 视图模型
    /// </summary>
    public TViewModel ViewModel { get; }



    /// <summary>
    /// 更新中
    /// </summary>
    public event Action<TViewModel>? Updating;

    /// <summary>
    /// 更新完毕
    /// </summary>
    public event Action<TViewModel>? Updated;

    /// <summary>
    /// 发生错误
    /// </summary>
    public event Action<Exception>? Faulted;


    public AutoUpdateBuilder(DbContext dbContext, IMapper mapper, TViewModel viewModel)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        ViewModel = viewModel;

        _propertyChangedEventHandler = async void (_, _) =>
        {
            try
            {
                if (IsStop) return;

                viewModel.AssertValidate();

                Updating?.Invoke(viewModel);

                await UpdateAsync(_cancellationTokenSource.Token);

                Updated?.Invoke(viewModel);
            }
            catch (Exception ex)
            {
                Faulted?.Invoke(ex);
            }
        };

        viewModel.PropertyChanged += _propertyChangedEventHandler;
    }


    public IAutoUpdateBuilder<TViewModel> OnUpdating(Action<TViewModel> action)
    {
        Updating += action;
        return this;
    }

    public IAutoUpdateBuilder<TViewModel> OnUpdated(Action<TViewModel> action)
    {
        Updated += action;
        return this;
    }

    public IAutoUpdateBuilder<TViewModel> OnFaulted(Action<Exception> action)
    {
        Faulted += action;
        return this;
    }

    public void Dispose()
    {
        IsStop = true;
        Updating = null;
        Updated = null;
        Faulted = null;

        ViewModel.PropertyChanged -= _propertyChangedEventHandler;
        _propertyChangedEventHandler = null;
    }


    async partial void OnIsStopChanged(bool value)
    {
        if (value)
        {
            _cancellationTokenSource = new();
        }
        else
        {
            await _cancellationTokenSource.CancelAsync();
        }
    }

    private async ValueTask UpdateAsync(CancellationToken cancellation = default)
    {
        var transaction = await _dbContext.Database.BeginTransactionAsync(cancellation);

        try
        {
            var entity = await _dbContext.Set<TEntity>().FirstOrDefaultAsync(x => x.Id == ViewModel.Id, cancellationToken: cancellation);
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity), "数据库不存在相同Id数据, 无法更新");
            }

            _mapper.Map(ViewModel, entity);

            await _dbContext.SaveChangesAsync(cancellation);
            await transaction.CommitAsync(cancellation);
        }
        catch (Exception ex)
        {
            transaction.Dispose();
            throw new DatabaseException("数据更新失败", ex);
        }
    }
}