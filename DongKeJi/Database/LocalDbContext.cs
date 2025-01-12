using System.IO;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace DongKeJi.Database;


/// <summary>
/// 本地数据库
/// </summary>
public class LocalDbContext : DbContext
{
    private readonly string _dbFolder;

    /// <summary>
    /// </summary>
    /// <param name="application"></param>
    /// <param name="name">数据库名称 (建议使用大驼峰命名, 不需要加扩展名</param>
    /// <exception cref="Exception"></exception>
    public LocalDbContext(IApplication application, string name)
    {
        _dbFolder = application.DatabaseDirectory;
        Name = name.Trim();
        if (ContextDictionary.TryGetValue(Name, out var _)) throw new Exception($"数据库名称冲突: {Name}");

        Database.EnsureCreated();
        ContextDictionary[name] = this;
    }


    public string Name { get; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!Directory.Exists(_dbFolder))
            Directory.CreateDirectory(_dbFolder);

        var path = Path.Combine(_dbFolder, $"{Name}.db");
        optionsBuilder.UseSqlite($"Data Source={path}");

        base.OnConfiguring(optionsBuilder);
    }

    #region --静态--

    private static readonly Dictionary<string, DbContext> ContextDictionary = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    ///     所有上下文
    /// </summary>
    public static IEnumerable<DbContext> Contexts => ContextDictionary.Values;

    /// <summary>
    ///     获取指定类型的
    /// </summary>
    /// <typeparam name="TDbContext"></typeparam>
    /// <returns></returns>
    public static DbContext? Get<TDbContext>()
        where TDbContext : DbContext
    {
        return ContextDictionary.Values.OfType<TDbContext>().FirstOrDefault();
    }


    static LocalDbContext()
    {
        Batteries.Init();
    }

    #endregion
}