namespace DongKeJi.Extensions;

public static class CSharpExtensions
{
    /// <summary>
    /// 概率生成真值
    /// </summary>
    /// <param name="random"></param>
    /// <param name="probability">概率</param>
    /// <returns></returns>
    public static bool NextBoolean(this Random random, double probability = 0.5)
    {
        return random.NextDouble() < probability;
    }


    /// <summary>
    /// 比较两个时间的年月是否一样
    /// </summary>
    /// <returns></returns>
    public static bool EqualsYearMonth(this DateTime a, DateTime b)
    {
        return a.Year == b.Year && a.Month == b.Month;
    }
}