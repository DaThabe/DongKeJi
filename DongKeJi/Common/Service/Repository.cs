using System.ComponentModel;
using AutoMapper;
using DongKeJi.Common.Entity;
using DongKeJi.Common.UI;
using DongKeJi.Common.Validation;
using DongKeJi.Common.ViewModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Wpf.Ui;

namespace DongKeJi.Common.Service;


public static class RepositoryExtensions
{
    /// <summary>
    /// 保存
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    /// <exception cref="DbUpdateException"></exception>
    public static async ValueTask AssertSaveSuccessAsync<TDbContext>(this TDbContext dbContext, CancellationToken cancellation = default)
        where TDbContext : DbContext
    {
        var result = await dbContext.SaveChangesAsync(cancellation);
        if(result < 0) throw new DbUpdateException("数据未写入数据库");
    }


    public static IAutoUpdateHandel<TViewModel> AutoUpdate<TEntity, TViewModel>(this DbContext dbContext, TViewModel viewModel, IMapper mapper)
        where TEntity : EntityBase
        where TViewModel : IViewModel, IIdentifiable
    {
        return new AutoUpdateHandel<TEntity, TViewModel>(dbContext, mapper, viewModel);
    }

    public static IAutoUpdateHandel<TViewModel> AutoUpdate<TEntity, TViewModel>(this DbContext dbContext, TEntity entity, IMapper mapper)
        where TEntity : EntityBase
        where TViewModel : IViewModel, IIdentifiable
    {
        var viewModel = mapper.Map<TViewModel>(entity);
        return dbContext.AutoUpdate<TEntity, TViewModel>(viewModel, mapper);
    }


    /// <summary>
    /// 注册自动保存
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TViewModel"></typeparam>
    /// <param name="dbContext"></param>
    /// <param name="entity"></param>
    /// <param name="services"></param>
    public static TViewModel RegisterAutoUpdate<TEntity, TViewModel>(
        this DbContext dbContext,
        TEntity entity,
        IServiceProvider services)
        where TEntity : EntityBase
        where TViewModel : DataViewModel
    {
        var mapper = services.GetRequiredService<IMapper>();
        var viewModel = mapper.Map<TViewModel>(entity);

        return dbContext.RegisterAutoUpdate<TEntity, TViewModel>(viewModel, services);
    }


    /// <summary>
    /// 注册自动保存
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TViewModel"></typeparam>
    /// <param name="dbContext"></param>
    /// <param name="viewModel"></param>
    /// <param name="services"></param>
    public static TViewModel RegisterAutoUpdate<TEntity, TViewModel>(
        this DbContext dbContext,
        TViewModel viewModel,
        IServiceProvider services)
        where TEntity : EntityBase
        where TViewModel : DataViewModel
    {
        var mapper = services.GetRequiredService<IMapper>();
        var snackbar = services.GetRequiredService<ISnackbarService>();

        var handel =  dbContext.AutoUpdate<TEntity, TViewModel>(viewModel, mapper);
        handel.Faulted += ex => snackbar.ShowError(ex);

        return handel.ViewModel;
    }




    /// <summary>
    /// 开启事务, 确保数据原子性
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="action"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    public static async ValueTask UnitOfWorkAsync(
        this DbContext dbContext, 
        Func<IDbContextTransaction, ValueTask> action, 
        CancellationToken cancellation = default)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellation);

        try
        {
            await action(transaction);
            await transaction.CommitAsync(cancellation);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellation);
            throw;
        }
    }

    /// <summary>
    /// 开启事务, 确保数据原子性
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="action"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    public static async ValueTask<TResult> UnitOfWorkAsync<TResult>(
        this DbContext dbContext, 
        Func<IDbContextTransaction, ValueTask<TResult>> action, 
        CancellationToken cancellation = default)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellation);

        try
        {
            var result = await action(transaction);
            await transaction.CommitAsync(cancellation);

            return result;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellation);
            throw;
        }
    }



    
}

public interface IAutoUpdateHandel<out TViewModel> : IDisposable
    where TViewModel : IViewModel, IIdentifiable
{
    /// <summary>
    /// 更新中
    /// </summary>
    event Action<TViewModel>? Updating;

    /// <summary>
    /// 更新完毕
    /// </summary>
    event Action<TViewModel>? Updated;

    /// <summary>
    /// 发生错误
    /// </summary>
    event Action<Exception>? Faulted;


    /// <summary>
    /// 是否暂停更新
    /// </summary>
    bool IsStop { get; set; }

    /// <summary>
    /// 视图模型
    /// </summary>
    TViewModel ViewModel { get; }
}


public class AutoUpdateHandel<TEntity, TViewModel> : IAutoUpdateHandel<TViewModel>
    where TEntity : EntityBase
    where TViewModel : IViewModel, IIdentifiable
{
    private TViewModel? _backup;
    private PropertyChangedEventHandler? _propertyChangedEventHandler;


    /// <summary>
    /// 是否暂停更新
    /// </summary>
    public bool IsStop { get; set; }

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


    public AutoUpdateHandel(DbContext dbContext, IMapper mapper, TViewModel viewModel)
    {
        ViewModel = viewModel;

        _propertyChangedEventHandler = async void (_, _) =>
        {
            try
            {
                if (IsStop) return;

                Updating?.Invoke(viewModel);

                await UpdateAsync(dbContext, mapper, viewModel, CancellationToken.None);

                Updated?.Invoke(viewModel);
            }
            catch (Exception ex)
            {
                Faulted?.Invoke(ex);
            }
        };

        viewModel.PropertyChanging += (_, _) => _backup = mapper.Map<TViewModel>(viewModel);
        viewModel.PropertyChanged += _propertyChangedEventHandler;
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




    /// <summary>
    /// 更新过程
    /// </summary>
    /// <param name="mapper"></param>
    /// <param name="viewModel"></param>
    /// <param name="cancellation"></param>
    /// <param name="dbContext"></param>
    /// <returns></returns>
    private static async ValueTask UpdateAsync(DbContext dbContext, IMapper mapper, IIdentifiable viewModel, CancellationToken cancellation = default)
    {
        //查询数据库
        var existEntity = await dbContext
            .Set<TEntity>()
            .FirstOrDefaultAsync(x => x.Id == viewModel.Id, cancellation);

        if (existEntity is null || existEntity.IsEmpty())
        {
            //新增数据
            var entity = mapper.Map<TEntity>(viewModel);
            dbContext.Add(entity);
        }
        else
        {
            //更新数据
            mapper.Map(viewModel, existEntity);
        }

        //保存到数据库
        await dbContext.AssertSaveSuccessAsync(cancellation);
    }
}