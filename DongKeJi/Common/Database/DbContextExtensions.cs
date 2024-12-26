using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace DongKeJi.Common.Database;

public static class DbContextExtensions
{
    /// <summary>
    /// 确保事务原子性
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="action"></param>
    /// <param name="exceptionCallback"></param>
    /// <returns></returns>
    public static async ValueTask<bool> UnitOfWorkAsync(
        this DbContext dbContext, 
        Func<IDbContextTransaction, Task> action,
        Action<Exception>? exceptionCallback = null)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        
        try
        {
            await action(transaction);
            await transaction.CommitAsync();

            return true;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            exceptionCallback?.Invoke(ex);
            return false;
        }
    }

    /// <summary>
    /// 确保事务原子性
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="action"></param>
    /// <param name="exceptionCallback"></param>
    /// <returns></returns>
    public static async ValueTask<TResult?> UnitOfWorkAsync<TResult>(
        this DbContext dbContext, 
        Func<IDbContextTransaction, ValueTask<TResult>> action, 
        Action<Exception>? exceptionCallback = null)
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

}
