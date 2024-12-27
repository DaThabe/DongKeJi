using System.IO;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace DongKeJi.Common.Database;

public class DatabaseContext : DbContext
{
    /// <summary>
    /// </summary>
    /// <param name="name">数据库名称 (建议使用大驼峰命名, 不需要加扩展名</param>
    /// <exception cref="Exception"></exception>
    public DatabaseContext(string name)
    {
        Name = name.Trim();
        if (_contexts.TryGetValue(Name, out var value)) throw new Exception($"数据库名称冲突: {Name}");

        Database.EnsureCreated();
        _contexts[name] = this;
    }


    public string Name { get; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!Directory.Exists(GlobalConfig.DatabaseDirectory))
            Directory.CreateDirectory(GlobalConfig.DatabaseDirectory);

        var path = Path.Combine(GlobalConfig.DatabaseDirectory, $"{Name}.db");
        optionsBuilder.UseSqlite($"Data Source={path}");

        base.OnConfiguring(optionsBuilder);
    }

    #region --静态--

    private static readonly Dictionary<string, DbContext> _contexts = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    ///     所有上下文
    /// </summary>
    public static IEnumerable<DbContext> Contexts => _contexts.Values;

    /// <summary>
    ///     获取指定类型的
    /// </summary>
    /// <typeparam name="TDbContext"></typeparam>
    /// <returns></returns>
    public static DbContext? Get<TDbContext>()
        where TDbContext : DbContext
    {
        return _contexts.Values.OfType<TDbContext>().FirstOrDefault();
    }


    static DatabaseContext()
    {
        Batteries.Init();
    }

    #endregion
}