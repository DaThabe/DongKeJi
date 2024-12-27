using System.Text;
using DongKeJi.Common.Entity;
using Microsoft.EntityFrameworkCore;

namespace DongKeJi.Common.Exceptions;

/// <summary>
/// 储存库异常
/// </summary>
public class RepositoryException : Exception
{
    public RepositoryActionType Action { get; }

    public RepositoryExceptionType Exception { get; }

    public object[] Args { get; }

    public override string Message { get; }



    public RepositoryException(RepositoryActionType action, RepositoryExceptionType exception, params object?[] args)
        :this(action, exception, null, args)
    {
        
    }

    public RepositoryException(RepositoryActionType action, RepositoryExceptionType exception, Exception? innerException = null, params object?[] args)
        :base(null, innerException)
    {
        Action = action;
        Exception = exception;
        Args = args.Where(x=> x != null).Select(x => x!).ToArray();

        //未知操作时发生错误, 原因是
        StringBuilder sb = new();
        sb.Append($"{ToString(Action)}失败, {ToString(Exception)}");

        if (Args.Length > 0)
        {
            sb.Append(", 参数: ");

            foreach (var i in Args)
            {
                sb.Append(i);
            }
        }

        Message = sb.ToString();
    }


    private static string ToString(RepositoryActionType type) => type switch
    {
        RepositoryActionType.Add => "添加",
        RepositoryActionType.Find => "查询",
        RepositoryActionType.Get => "获取",
        RepositoryActionType.Remove => "删除",
        RepositoryActionType.Update => "更新",
        _ => "操作"
    };

    private static string ToString(RepositoryExceptionType type) => type switch
    {
        RepositoryExceptionType.SaveFailed => "保存未成功",
        RepositoryExceptionType.PrimaryKeyConflict => "数据已存在",
        RepositoryExceptionType.PrimaryKeyMissing => "数据不存在",
        _ => "未知原因"
    };
}

public static class RepositoryExceptionExtensions
{
    /// <summary>
    /// 抛出主键冲突异常 (如果传入的实体不为 <see cref="Nullable"/>, <see cref="Identifiable.Empty"/>则视为冲突
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="entity"></param>
    /// <param name="action"></param>
    /// <param name="args"></param>
    /// <exception cref="RepositoryException"></exception>
    public static void IfThrowPrimaryKeyConflict<TEntity>(this TEntity? entity, RepositoryActionType action, params object?[] args)
        where TEntity : IIdentifiable
    {
        if (entity != null)
        {
            throw new RepositoryException(action, RepositoryExceptionType.PrimaryKeyConflict, args);
        }
    }

    /// <summary>
    /// 抛出主键缺失异常 (如果传入的实体为null则视为缺失
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="entity"></param>
    /// <param name="action"></param>
    /// <param name="args"></param>
    /// <exception cref="RepositoryException"></exception>
    public static TEntity IfThrowPrimaryKeyMissing<TEntity>(this TEntity? entity, RepositoryActionType action, params object?[] args)
        where TEntity : IIdentifiable
    {
        if (entity == null)
        {
            throw new RepositoryException(action, RepositoryExceptionType.PrimaryKeyMissing, args);
        }

        return entity;
    }

    /// <summary>
    /// 抛出外键缺失异常
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="entity"></param>
    /// <param name="action"></param>
    /// <param name="args"></param>
    /// <exception cref="RepositoryException"></exception>
    public static void IfThrowForeignKeyMissing<TEntity>(this TEntity entity, RepositoryActionType action, params object?[] args)
        where TEntity : IIdentifiable
    {
        throw new RepositoryException(action, RepositoryExceptionType.ForeignKeyMissing, args);
    }



    /// <summary>
    /// 调用 <see cref="DbContext.SaveChangesAsync"/> 并判断是否有保存结果, 或者发生异常
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="action"></param>
    /// <param name="args"></param>
    /// <exception cref="RepositoryException"></exception>
    public static async ValueTask IfThrowSaveFailedAsync<TDbContext>(this TDbContext dbContext, RepositoryActionType action, params object?[] args)
        where TDbContext : DbContext
    {
        await dbContext.IfThrowSaveFailedAsync(action, CancellationToken.None, args);
    }

    /// <summary>
    /// 调用 <see cref="DbContext.SaveChangesAsync"/> 并判断是否有保存结果, 或者发生异常
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="action"></param>
    /// <param name="cancellation"></param>
    /// <param name="args"></param>
    /// <exception cref="RepositoryException"></exception>
    public static async ValueTask IfThrowSaveFailedAsync<TDbContext>(this TDbContext dbContext, RepositoryActionType action, CancellationToken cancellation, params object[] args)
        where TDbContext : DbContext
    {
        try
        {
            if (await dbContext.SaveChangesAsync(cancellation) < 0)
            {
                throw new RepositoryException(action, RepositoryExceptionType.SaveFailed);
            }
        }
        catch (RepositoryException)
        {
            throw;
        }
        catch (Exception e)
        {
            throw new RepositoryException(action, RepositoryExceptionType.SaveFailed, innerException: e);
        }
    }

}