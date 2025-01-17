using DongKeJi.Entity;
using Microsoft.EntityFrameworkCore;

namespace DongKeJi.Config;


/// <summary>
/// 包含配置的数据库
/// </summary>
public interface IConfigDbContext
{
    /// <summary>
    /// 配置表
    /// </summary>
    DbSet<ConfigEntity> Config { get; }
}