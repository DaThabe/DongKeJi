namespace DongKeJi.Common.Exceptions;

/// <summary>
/// 储存库异常
/// </summary>
public class RepositoryException : Exception
{

    public RepositoryException(string message) : base(message)
    {

    }

    public RepositoryException(string message, Exception innerException) : base(message, innerException)
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
        if (entity is not null && !entity.IsEmpty())
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
        if (entity is null || entity.IsEmpty())
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