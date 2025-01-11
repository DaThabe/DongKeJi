using AutoMapper;
using DongKeJi.Entity;
using DongKeJi.ViewModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace DongKeJi.Database;


public static class DbContextExtensions
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
        if (result < 0) throw new DbUpdateException("数据未写入数据库");
    }


    /// <summary>
    /// 构建自动更新
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TViewModel"></typeparam>
    /// <param name="dbContext"></param>
    /// <param name="viewModel"></param>
    /// <param name="mapper"></param>
    /// <returns></returns>
    public static IAutoUpdateBuilder<TViewModel> AutoUpdate<TEntity, TViewModel>(this DbContext dbContext, TViewModel viewModel, IMapper mapper)
        where TEntity : EntityBase
        where TViewModel : IEntityViewModel
    {
        return new AutoUpdateBuilder<TEntity, TViewModel>(dbContext, mapper, viewModel);
    }
    
    /// <summary>
    /// 构建自动更新
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TViewModel"></typeparam>
    /// <param name="dbContext"></param>
    /// <param name="entity"></param>
    /// <param name="mapper"></param>
    /// <returns></returns>
    public static IAutoUpdateBuilder<TViewModel> AutoUpdate<TEntity, TViewModel>(this DbContext dbContext, TEntity entity, IMapper mapper)
        where TEntity : EntityBase
        where TViewModel : IEntityViewModel
    {
        var viewModel = mapper.Map<TViewModel>(entity);
        return dbContext.AutoUpdate<TEntity, TViewModel>(viewModel, mapper);
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