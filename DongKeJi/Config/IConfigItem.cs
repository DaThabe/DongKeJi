using DongKeJi.Database;
using DongKeJi.Entity;
using DongKeJi.Exceptions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace DongKeJi.Config;


/// <summary>
/// 配置元素
/// </summary>
/// <typeparam name="TValue"></typeparam>
public interface IConfigItem<TValue>
{
    /// <summary>
    /// 配置值
    /// </summary>
    ValueTask<TValue> GetAsync(
        bool useBuffer = true, 
        CancellationToken cancellation = default);

    /// <summary>
    /// 配置值
    /// </summary>
    ValueTask SetAsync(
        TValue? value, 
        CancellationToken cancellation = default);

    /// <summary>
    /// 更新值到数据库
    /// </summary>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    ValueTask UpdateAsync(CancellationToken cancellation = default);
}


public class ConfigItem<TDbContext, TValue>(string key, TDbContext dbContext) :  IConfigItem<TValue>
    where TDbContext : DbContext, IConfigDbContext
{
    /// <summary>
    /// 键
    /// </summary>
    public string Key { get; } = key;



    public async ValueTask<TValue> GetAsync(bool useBuffer = true, CancellationToken cancellation = default)
    {
        try
        {
            if (!useBuffer || _value is null)
            {
                await ReloadAsync(cancellation);
            }

            ArgumentNullException.ThrowIfNull(_value);
            return _value;
        }
        catch (ConfigException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ConfigException($"配置获取失败\n键: {Key}\n值: {_value}\n值类型: {typeof(TValue).Name}", ex);
        }
    }

    public async ValueTask SetAsync(TValue? value, CancellationToken cancellation = default)
    {
        try
        {
            _value = value;
            await UpdateAsync(cancellation);
        }
        catch (ConfigException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ConfigException($"配置设置失败\n键: {Key}\n值: {value}\n值类型: {typeof(TValue).Name}", ex);
        }
    }

    public async ValueTask UpdateAsync(CancellationToken cancellation = default)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellation);

        try
        {
            var jsonStringValue = JsonConvert.SerializeObject(_value);

            var configEntity = await dbContext.Configs.FirstOrDefaultAsync(x => x.Key == Key, cancellationToken: cancellation);

            if (configEntity is null)
            {
                configEntity = new ConfigEntity
                {
                    Key = Key,
                    JsonStringValue = jsonStringValue
                };

                dbContext.Add(configEntity);
            }
            else
            {
                configEntity.JsonStringValue = jsonStringValue;
            }

            await dbContext.AssertSaveSuccessAsync(cancellation: cancellation);
            await transaction.CommitAsync(cancellation);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellation);
            throw new ConfigException($"配置更新失败\n键: {Key}\n值: {_value}\n值类型: {typeof(TValue).Name}", ex);
        }
    }


    private async ValueTask ReloadAsync(CancellationToken cancellation = default)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellation);

        try
        {
            var configEntity = await dbContext.Configs.FirstOrDefaultAsync(x => x.Key == Key, cancellationToken: cancellation);
            configEntity = DatabaseException.ThrowIfEntityNotFound(configEntity);

            var value = JsonConvert.DeserializeObject<TValue>(configEntity.JsonStringValue);
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value), "配置转换失败或未空");
            }

            _value = value;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellation);
            throw new ConfigException($"配置重载失败\n键: {Key}\n值: {_value}\n值类型: {typeof(TValue).Name}", ex);
        }
    }


    private TValue? _value;
}
