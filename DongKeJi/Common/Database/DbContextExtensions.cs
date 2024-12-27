using DongKeJi.Common.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace DongKeJi.Common.Database;

public static class DbContextExtensions
{
    /// <summary>
    ///     确保事务原子性
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="action"></param>
    /// <param name="exceptionCallback"></param>
    /// <returns></returns>
    [Obsolete]
    public static async ValueTask<T?> UnitOfWorkAsync<T>(
        this DbContext dbContext,
        Func<IDbContextTransaction, ValueTask<T>> action,
        Action<Exception>? exceptionCallback)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync();

        try
        {
            var result = await action(transaction);
            await transaction.CommitAsync();
            return result;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            exceptionCallback?.Invoke(ex);
            return default;
        }
    }


    /// <summary>
    ///     确保事务原子性 (如果发生异常则回退所有执行过程, 异常之后继续抛出
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    [Obsolete]
    public static async ValueTask<T> UnitOfWorkAsync<T>(
        this DbContext dbContext,
        Func<IDbContextTransaction, ValueTask<T>> action)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync();

        try
        {
            var result = await action(transaction);
            await transaction.CommitAsync();

            return result;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    [Obsolete]
    public static async ValueTask UnitOfWorkAsync(
        this DbContext dbContext,
        Func<IDbContextTransaction, ValueTask> action)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync();

        try
        {
            await action(transaction);
            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}