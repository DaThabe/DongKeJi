using AutoMapper;
using DongKeJi.Common.Database;
using DongKeJi.Common.Entity;
using DongKeJi.Common.ViewModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DongKeJi.Common.Service;


/// <summary>
/// 仓库
/// </summary>
/// <typeparam name="TDto"></typeparam>
public interface IRepository<TDto>
    where TDto : IIdentifiable
{
    /// <summary>
    /// 更新
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    ValueTask<bool> UpdateAsync(TDto dto, CancellationToken cancellation = default);

    /// <summary>
    /// 添加
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    ValueTask<bool> AddAsync(TDto dto, CancellationToken cancellation = default);

    /// <summary>
    /// 删除
    /// </summary>
    /// <param name="token"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    ValueTask<bool> DeleteAsync(IIdentifiable token, CancellationToken cancellation = default);

    /// <summary>
    /// 查询
    /// </summary>
    /// <param name="token"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    ValueTask<TDto?> FindByIdAsync(IIdentifiable token, CancellationToken cancellation = default);

    /// <summary>
    /// 获取所有
    /// </summary>
    /// <returns></returns>
    ValueTask<IEnumerable<TDto>> GetAllAsync(CancellationToken cancellation = default);

    /// <summary>
    /// 获取所有
    /// </summary>
    /// <returns></returns>
    ValueTask<IEnumerable<TDto>> GetAllAsync(int skip, int count, CancellationToken cancellation = default);
}


public abstract class Repository<TDbContext, TEntity, TDto>(IServiceProvider services) : IRepository<TDto>, IInfrastructure
    where TDbContext : DbContext
    where TEntity : EntityBase
    where TDto : IIdentifiable
{
    protected abstract IQueryable<TEntity> Queryable { get; }

    public IServiceProvider ServiceProvider => services;
    protected TDbContext DbContext => services.GetRequiredService<TDbContext>();
    protected IMapper Mapper => services.GetRequiredService<IMapper>();
    protected ILogger Logger => services.GetRequiredService<ILogger<Repository<TDbContext, TEntity, TDto>>>();


    public async ValueTask<bool> AddAsync(TDto dto, CancellationToken cancellation = default)
    {
        return await DbContext.UnitOfWorkAsync(async x =>
        {
            var entity = Mapper.Map<TEntity>(dto);

            await DbContext.AddAsync(entity, cancellation);
            var result = await DbContext.SaveChangesAsync(cancellation);

            return result > 0;

        }, ex => Logger.LogError(ex, "添加数据时发生错误"));
    }

    public async ValueTask<bool> UpdateAsync(TDto dto, CancellationToken cancellation = default)
    {
        return await DbContext.UnitOfWorkAsync(async x =>
        {
            var entity = await Queryable.FirstOrDefaultAsync(e => e.Id == dto.Id, cancellationToken: cancellation);

            if (entity is null)
            {
                Logger.LogWarning("更新失败, 未查询到实体: {dto}", dto);
                return false;
            }

            Mapper.Map(dto, entity);
            var result = await DbContext.SaveChangesAsync(cancellation);

            return result > 0;

        }, ex => Logger.LogError(ex, "更新数据时发生错误"));
    }

    public async ValueTask<bool> DeleteAsync(IIdentifiable token, CancellationToken cancellation = default)
    {
        return await DbContext.UnitOfWorkAsync(async x =>
        {
            var entity = await Queryable.FirstOrDefaultAsync(e => e.Id == token.Id, cancellationToken: cancellation);
            if (entity is null)
            {
                Logger.LogWarning("数据删除失败, 无符合Id");
                return false;
            }

            DbContext.Remove(entity);
            await DbContext.SaveChangesAsync(cancellation);

            return true;

        }, ex => Logger.LogError(ex, "删除数据时发生错误"));
    }

    public async ValueTask<TDto?> FindByIdAsync(IIdentifiable token, CancellationToken cancellation = default)
    {
        return await DbContext.UnitOfWorkAsync(async x =>
        {
            var entity = await Queryable.FirstOrDefaultAsync(e => e.Id == token.Id, cancellationToken: cancellation);

            if (entity is null)
            {
                Logger.LogWarning("查询数据失败, 未符合条件的Id: {id}", token.Id);
                return default;
            }

            return EntityToDto(entity);

        }, ex => Logger.LogError(ex, "查询数据时发生错误"));
    }

    public async ValueTask<IEnumerable<TDto>> GetAllAsync(CancellationToken cancellation = default)
    {
        return await DbContext.UnitOfWorkAsync(async x =>
        {
            var entityList = await Queryable.ToListAsync(cancellationToken: cancellation);
            return EntityToDto(entityList).ToArray();

        }, ex => Logger.LogError(ex, "获取数据时发生错误")) ?? [];
    }

    public async ValueTask<IEnumerable<TDto>> GetAllAsync(int skip, int count, CancellationToken cancellation = default)
    {
        return await DbContext.UnitOfWorkAsync(async x =>
        {
            var entityList = await Queryable.Skip(skip).Take(count).ToListAsync(cancellationToken: cancellation);
            return EntityToDto(entityList).ToArray();

        }, ex => Logger.LogError(ex, "获取数据时发生错误")) ?? [];
    }


    protected virtual TDto EntityToDto(TEntity entity)
    {
        var dto = Mapper.Map<TDto>(entity);
        return dto;
    }

    protected virtual IEnumerable<TDto> EntityToDto(IEnumerable<TEntity> entitys)
    {
        foreach (var i in entitys) yield return EntityToDto(i);
    }
}