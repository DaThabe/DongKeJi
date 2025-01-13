using DongKeJi.Core.Model;
using DongKeJi.Core.Model.Entity;
using DongKeJi.Database;
using DongKeJi.Exceptions;
using DongKeJi.Inject;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DongKeJi.Core.Service;

public interface IConfigService
{
    /// <summary>
    /// 获取值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    ValueTask<T> GetAsync<T>(
        string key,
        CancellationToken cancellation = default);

    /// <summary>
    /// 获取并且确保存在,如果不存在则用传入的值保存到数据库
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    ValueTask<T> GetOrEnsureAsync<T>(
        string key,
        T value,
        CancellationToken cancellation = default);

    /// <summary>
    /// 设置值
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    ValueTask SetAsync<T>(
        string key,
        T value,
        CancellationToken cancellation = default);

    /// <summary>
    /// 删除键值
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    ValueTask RemoveAsync(
        string key,
        CancellationToken cancellation = default);
}


[Inject(ServiceLifetime.Singleton, typeof(IConfigService))]
internal class ConfigService(CoreDbContext dbContext) : IConfigService
{
    public async ValueTask<T> GetAsync<T>(string key, CancellationToken cancellation = default)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellation);

        try
        {
            var searchKey = key.ToLower();
            var configEntity = await dbContext.Configs.FirstOrDefaultAsync(x => x.Key == searchKey, cancellationToken: cancellation);

            if (configEntity is null)
            {
                throw new ArgumentNullException(nameof(configEntity), $"指定键不存在\n键: {key}");
            }

            var value = JsonConvert.DeserializeObject<T>(configEntity.JsonStringValue);
            if (value is not null) return value;
            
            throw new ArgumentNullException(nameof(value), "值转换失败");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellation);
            throw new DatabaseException($"配置获取失败\n键: {key}\n值类型: {typeof(T).Name}", ex);
        }
    }

    public async ValueTask<T> GetOrEnsureAsync<T>(string key, T value, CancellationToken cancellation = default)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellation);

        try
        {
            var searchKey = key.ToLower();
            var configEntity = await dbContext.Configs.FirstOrDefaultAsync(x => x.Key == searchKey, cancellationToken: cancellation);

            if (configEntity is not null)
            {
                return value;
            }

            configEntity = new ConfigEntity
            {
                Key = searchKey,
                JsonStringValue = JsonConvert.SerializeObject(value)
            };

            dbContext.Add(configEntity);
            await dbContext.AssertSaveSuccessAsync(cancellation: cancellation);
            return value;

        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellation);
            throw new DatabaseException($"配置获取失败\n键: {key}\n值: {value}\n值类型: {typeof(T).Name}", ex);
        }
    }

    public async ValueTask SetAsync<T>(string key, T value, CancellationToken cancellation = default)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellation);

        try
        {
            var setKey = key.ToLower();
            var jsonStringValue = JsonConvert.SerializeObject(value);

            var configEntity = await dbContext.Configs.FirstOrDefaultAsync(x => x.Key == setKey, cancellationToken: cancellation);

            if (configEntity is null)
            {
                configEntity = new ConfigEntity
                {
                    Key = setKey,
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
            throw new DatabaseException($"配置设置失败\n键: {key}\n值: {value}\n值类型: {typeof(T).Name}", ex);
        }
    }

    public async ValueTask RemoveAsync(string key, CancellationToken cancellation = default)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellation);

        try
        {
            var setKey = key.ToLower();
            var configEntity = await dbContext.Configs.FirstOrDefaultAsync(x => x.Key == setKey, cancellationToken: cancellation);

            configEntity = DatabaseException.ThrowIfEntityNotFound(configEntity);

            dbContext.Remove(configEntity);
            await dbContext.AssertSaveSuccessAsync(cancellation: cancellation);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellation);
            throw new DatabaseException($"配置删除失败\n键: {key}", ex);
        }
    }
}