using System.Collections.ObjectModel;

namespace DongKeJi.Extensions;

public static class EnumerableExtensions
{
    public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> values)
    {
        return new ObservableCollection<T>(values);
    }


    public static IEnumerable<TEntity> SkipAndTake<TEntity>(this IEnumerable<TEntity> values, int? skip = null,
        int? take = null)
    {
        if (skip is > 0) values = values.Skip(skip.Value);

        if (take is > 0) values = values.Take(take.Value);


        return values;
    }


    /// <summary>
    ///     删除符合条件的元素
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="values"></param>
    /// <param name="matcher"></param>
    /// <returns>是否删除成功</returns>
    public static bool Remove<T>(this ICollection<T> values, Func<T, bool> matcher)
    {
        return values.RemoveAtMatchedIndex(matcher) != -1;
    }

    /// <summary>
    ///     删除元素并返回删除前的索引值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="values"></param>
    /// <param name="matcher"></param>
    /// <returns>如果是-1代表没有删除</returns>
    public static int RemoveAtMatchedIndex<T>(this ICollection<T> values, Func<T, bool> matcher)
    {
        var index = -1;

        foreach (var i in values)
        {
            index++;
            if (!matcher(i)) continue;

            values.Remove(i);
            return index;
        }

        return index;
    }

    /// <summary>
    ///     尝试根据给定的索引和可选的偏移量从 ObservableCollection 中获取元素。
    /// </summary>
    /// <typeparam name="T">集合中的元素类型。</typeparam>
    /// <param name="values">要检索元素的 ObservableCollection。</param>
    /// <param name="index">初始索引。</param>
    /// <param name="offset">可选的索引偏移量，默认为 0。如果偏移量为 0，将返回指定索引处的元素；否则，将返回带偏移量的元素。</param>
    /// <returns>
    ///     如果索引和偏移量的组合在集合范围内，返回相应的元素；
    ///     — 当索引超出集合范围时，返回第一个或最后一个元素；
    ///     — 如果偏移后的索引超出集合范围，返回<see langword="default" /> 值。
    /// </returns>
    public static T? TryGetElementWithOffset<T>(this IList<T> values, int index, int offset = 0)
    {
        try
        {
            var currentIndex = index + offset;

            if (currentIndex >= values.Count) return values.Last();

            if (currentIndex < 0) return values.First();

            return values[currentIndex];
        }
        catch
        {
            return default;
        }
    }


    /// <summary>
    ///     添加一个符合条件的元素 (如果集合中没有元素, 无论如何都会添加
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="values"></param>
    /// <param name="value"></param>
    /// <param name="matcher"></param>
    /// <returns>如果添加成功则True</returns>
    public static bool Add<T>(this ICollection<T> values, T value, Func<T, bool> matcher)
    {
        if (values.Count > 0)
            if (!values.Any(matcher))
                return false;

        values.Add(value);
        return true;
    }
}