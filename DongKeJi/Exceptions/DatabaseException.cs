namespace DongKeJi.Exceptions;

/// <summary>
/// 数据库异常
/// </summary>
public class DatabaseException : Exception
{

    public DatabaseException(string message) : base(message)
    {

    }

    public DatabaseException(string message, Exception innerException) : base(message, innerException)
    {

    }


    /// <summary>
    /// 实体已存在则抛出异常, 传入的实体不等于null且Id不等于Empty
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="entity"></param>
    /// <param name="message"></param>
    /// <exception cref="EntityAlreadyExistsException"></exception>
    public static void ThrowIfEntityAlreadyExists<TEntity>(TEntity? entity, string? message = null)
        where TEntity : IIdentifiable
    {
        if (entity is not null && !entity.IsNullOrEmpty())
        {
            throw new EntityAlreadyExistsException(message ?? $"实体已存在\nId: {entity.Id}");
        }
    }

    /// <summary>
    /// 根据为真的条件抛出实体存在异常
    /// </summary>
    /// <param name="isThrow"></param>
    /// <param name="message"></param>
    /// <exception cref="EntityAlreadyExistsException"></exception>
    public static void ThrowIfEntityAlreadyExists(bool isThrow, string? message = null)
    {
        if (isThrow)
        {
            throw new EntityAlreadyExistsException(message ?? "实体已存在");
        }
    }



    /// <summary>
    /// 实体不存在则抛异常, 传入的实体等于null或Id等于Empty
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="entity"></param>
    /// <returns></returns>
    /// <exception cref="EntityNotFoundException"></exception>
    public static TEntity ThrowIfEntityNotFound<TEntity>(TEntity? entity, string? message = null)
        where TEntity : IIdentifiable
    {
        if (entity is null || entity.IsNullOrEmpty())
        {
            throw new EntityNotFoundException(message ?? "实体不存在");
        }

        return entity;
    }

    public static void ThrowIfEntityNotFound(bool isThrow, string? message = null)
    {
        if (isThrow)
        {
            throw new EntityNotFoundException(message ?? "实体不存在");
        }
    }
}