using AutoMapper;
using DongKeJi.Common.Entity;
using DongKeJi.Common.Exceptions;
using DongKeJi.Common.ViewModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore.Metadata;

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
    /// <summary>
    /// 
    /// </summary>
    public IServiceProvider ServiceProvider => services;

    /// <summary>
    /// 实体转换器
    /// </summary>
    public IMapper Mapper => ServiceProvider.GetRequiredService<IMapper>();

    /// <summary>
    /// 数据库上下文
    /// </summary>
    public TDbContext DbContext => ServiceProvider.GetRequiredService<TDbContext>();

    /// <summary>
    /// 当前储存库数据表
    /// </summary>
    public DbSet<TEntity> DbSet => DbContext.Set<TEntity>();


    /// <summary>
    /// 注册视图模型自动更新
    /// </summary>
    /// <param name="vm"></param>
    /// <param name="exceptionCallback"></param>
    /// <returns></returns>
    protected TViewModel RegisterAutoUpdate(TViewModel vm, Action<Exception>? exceptionCallback = null)
    {
        return DbContext.RegisterAutoUpdate<TEntity, TViewModel>(vm, Mapper, exceptionCallback);
    }

    /// <summary>
    /// 注册视图模型可指定派生类
    /// </summary>
    /// <typeparam name="TViewModelChild"></typeparam>
    /// <param name="vm"></param>
    /// <param name="exceptionCallback"></param>
    /// <returns></returns>
    protected TViewModelChild RegisterAutoUpdate<TViewModelChild>(TViewModelChild vm, Action<Exception>? exceptionCallback = null)
        where TViewModelChild : TViewModel
    {
        return DbContext.RegisterAutoUpdate<TEntity, TViewModelChild>(vm, Mapper, exceptionCallback);
    }


    /// <summary>
    /// 将传入的实体转为视图模型后, 注册视图模型自动更新
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="exceptionCallback"></param>
    /// <returns></returns>
    protected TViewModel RegisterAutoUpdate(TEntity entity, Action<Exception>? exceptionCallback = null)
    {
        return RegisterAutoUpdate(Mapper.Map<TViewModel>(entity), exceptionCallback);
    }

    /// <summary>
    /// 将传入的实体转为视图模型后, 注册视图模型自动更新
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="exceptionCallback"></param>
    /// <returns></returns>
    protected TViewModelChild RegisterAutoUpdate<TViewModelChild>(TEntity entity, Action<Exception>? exceptionCallback = null)
        where TViewModelChild : TViewModel
    {
        return RegisterAutoUpdate(Mapper.Map<TViewModelChild>(entity), exceptionCallback);
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

public static class RepositoryExtensions
{
    /// <summary>
    /// 注册自动保存
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TViewModel"></typeparam>
    /// <param name="dbContext"></param>
    /// <param name="viewModel"></param>
    /// <param name="mapper"></param>
    /// <param name="exceptionCallback"></param>
    /// <param name="cancellation"></param>
    /// <exception cref="RepositoryException"></exception>
    public static TViewModel RegisterAutoUpdate<TEntity, TViewModel>(
        this DbContext dbContext,
        TViewModel viewModel, 
        IMapper mapper, 
        Action<Exception>? exceptionCallback = null,
        CancellationToken cancellation = default)
        where TEntity : EntityBase
        where TViewModel : IViewModel, IIdentifiable
    {
        viewModel.PropertyChanged += async (_, _) =>
        {
            try
            {
                await SaveProcessAsync();
            }
            catch (Exception e)
            {
                exceptionCallback?.Invoke(e);
            }
        };

        return viewModel;

        //保存过程
        async Task SaveProcessAsync()
        {
            var existEntity = await dbContext
                .Set<TEntity>()
                .FirstOrDefaultAsync(x => x.Id == viewModel.Id, cancellationToken: cancellation);

            if (existEntity is null || existEntity.IsEmpty())
            {
                var entity = mapper.Map<TEntity>(viewModel);
                dbContext.Add(entity);
            }
            else
            {
                mapper.Map(viewModel, existEntity);
            }

            if (await dbContext.SaveChangesAsync(cancellation) <= 0)
            {
                throw new RepositoryException($"写入数据库失败, 数据信息:{viewModel}");
            }
        }
    }


    /// <summary>
    /// 注册自动保存
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TViewModel"></typeparam>
    /// <param name="dbContext"></param>
    /// <param name="entity"></param>
    /// <param name="mapper"></param>
    /// <param name="exceptionCallback"></param>
    /// <param name="cancellation"></param>
    public static TViewModel RegisterAutoUpdate<TEntity, TViewModel>(
        this DbContext dbContext,
        TEntity entity,
        IMapper mapper,
        Action<Exception>? exceptionCallback = null,
        CancellationToken cancellation = default)
        where TEntity : EntityBase
        where TViewModel : IViewModel, IIdentifiable
    {
        var viewModel = mapper.Map<TViewModel>(entity);
        return dbContext.RegisterAutoUpdate<TEntity, TViewModel>(viewModel, mapper, exceptionCallback, cancellation);
    }
}