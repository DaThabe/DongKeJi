using AutoMapper;
using DongKeJi.Common.Entity;
using DongKeJi.Common.Exceptions;
using DongKeJi.Common.ViewModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace DongKeJi.Common.Service;


/// <summary>
/// 储存库
/// </summary>
/// <typeparam name="TDbContext"></typeparam>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TViewModel"></typeparam>
public abstract class Repository<TDbContext, TEntity,TViewModel>(IServiceProvider services) : IInfrastructure
    where TDbContext : DbContext
    where TEntity : EntityBase
    where TViewModel : IViewModel, IIdentifiable
{

    public IServiceProvider ServiceProvider => services;
    
    /// <summary>
    /// 实体转换器
    /// </summary>
    protected IMapper Mapper => ServiceProvider.GetRequiredService<IMapper>();
    
    /// <summary>
    /// 数据库上下文
    /// </summary>
    protected TDbContext DbContext => ServiceProvider.GetRequiredService<TDbContext>();

    /// <summary>
    /// 当前储存库数据表
    /// </summary>
    protected DbSet<TEntity> DbSet => DbContext.Set<TEntity>();




    /// <summary>
    /// 更新实体
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    private async ValueTask UpdateAsync(TEntity entity, CancellationToken cancellation = default)
    {
        var existEntity = await DbSet
            .FirstOrDefaultAsync(x => x.Id == entity.Id, cancellationToken: cancellation);

        if (existEntity is null || existEntity.IsEmpty())
        {
            DbContext.Add(entity);
        }
        else
        {
            Mapper.Map(entity, existEntity);
        }

        if (await DbContext.SaveChangesAsync(cancellation) < 0)
        {
            throw new RepositoryException($"写入数据库失败, 实体信息:{entity}");
        }
    }

    
    /// <summary>
    /// 注册视图模型自动更新
    /// </summary>
    /// <param name="vm"></param>
    /// <returns></returns>
    protected TViewModel RegisterAutoUpdate(TViewModel vm)
    {
        vm.PropertyChanged += async (_, _) =>
        {
            await UpdateAsync(Mapper.Map<TEntity>(vm));
        };

        return vm;
    }

    /// <summary>
    /// 注册视图模型可指定派生类
    /// </summary>
    /// <typeparam name="TViewModelChild"></typeparam>
    /// <param name="vm"></param>
    /// <returns></returns>
    protected TViewModelChild RegisterAutoUpdate<TViewModelChild>(TViewModelChild vm)
        where TViewModelChild : TViewModel
    {
        vm.PropertyChanged += async (_, _) =>
        {
            await UpdateAsync(Mapper.Map<TEntity>(vm));
        };

        return vm;
    }


    /// <summary>
    /// 将传入的实体转为视图模型后, 注册视图模型自动更新
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    protected TViewModel RegisterAutoUpdate(TEntity entity)
    {
        return RegisterAutoUpdate(Mapper.Map<TViewModel>(entity));
    }

    /// <summary>
    /// 将传入的实体转为视图模型后, 注册视图模型自动更新
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    protected TViewModelChild RegisterAutoUpdate<TViewModelChild>(TEntity entity)
        where TViewModelChild : TViewModel
    {
        return RegisterAutoUpdate(Mapper.Map<TViewModelChild>(entity));
    }



    /// <summary>
    /// 开启事务, 确保数据原子性
    /// </summary>
    /// <param name="action"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    protected async ValueTask UnitOfWorkAsync(Func<IDbContextTransaction, ValueTask> action, CancellationToken cancellation = default)
    {
        await using var transaction = await DbContext.Database.BeginTransactionAsync(cancellation);

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
    /// <param name="action"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    protected async ValueTask<TResult> UnitOfWorkAsync<TResult>(Func<IDbContextTransaction, ValueTask<TResult>> action, CancellationToken cancellation = default)
    {
        await using var transaction = await DbContext.Database.BeginTransactionAsync(cancellation);

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