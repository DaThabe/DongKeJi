namespace DongKeJi.Extensions;

public static class QueryableExtensions
{
    /// <summary>
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="queryable"></param>
    /// <param name="skip"></param>
    /// <param name="take"></param>
    /// <returns></returns>
    public static IQueryable<TEntity> SkipAndTake<TEntity>(this IQueryable<TEntity> queryable, int? skip = null,
        int? take = null)
    {
        if (skip is > 0) queryable = queryable.Skip(skip.Value);

        if (take is > 0) queryable = queryable.Take(take.Value);


        return queryable;
    }
}