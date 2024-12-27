using AutoMapper;
using DongKeJi.Common.Entity;
using DongKeJi.Common.Exceptions;
using DongKeJi.Common.ViewModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace DongKeJi.Common.Service;


/// <summary>
/// 储存库
/// </summary>
/// <typeparam name="TDbContext"></typeparam>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TViewModel"></typeparam>
public abstract class Repository<TDbContext, TEntity,TViewModel>(IServiceProvider services) : IInfrastructure
    where TDbContext : DbContext
    where TEntity : EntityBase
    where TViewModel : IViewModel, IIdentifiable
{

    public IServiceProvider ServiceProvider => services;
    
    /// <summary>
    /// 实体转换器
    /// </summary>
    protected IMapper Mapper => ServiceProvider.GetRequiredService<IMapper>();
    
    /// <summary>
    /// 数据库上下文
    /// </summary>
    protected TDbContext DbContext => ServiceProvider.GetRequiredService<TDbContext>();

    /// <summary>
    /// 当前储存库数据表
    /// </summary>
    protected DbSet<TEntity> Table => DbContext.Set<TEntity>();


    /// <summary>
    /// 更新实体
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    private async ValueTask UpdateAsync(TEntity entity, CancellationToken cancellation = default)
    {
        var existEntity = await Table
            .FirstOrDefaultAsync(x => x.Id == entity.Id, cancellationToken: cancellation);

        existEntity.IfThrowPrimaryKeyMissing(RepositoryActionType.Update, entity);
        
        Mapper.Map(entity, existEntity);
        
        await DbContext.IfThrowSaveFailedAsync(RepositoryActionType.Update, cancellation, entity);
    }


    /// <summary>
    /// 注册视图模型自动更新
    /// </summary>
    /// <param name="vm"></param>
    /// <returns></returns>
    protected TViewModel RegisterAutoUpdate(TViewModel vm)
    {
        vm.PropertyChanged += async (_, _) =>
        {
            await UpdateAsync(Mapper.Map<TEntity>(vm));
        };

        return vm;
    }

    /// <summary>
    /// 将传入的实体转为视图模型后, 注册视图模型自动更新
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    protected TViewModel RegisterAutoUpdate(TEntity entity)
    {
        return RegisterAutoUpdate(Mapper.Map<TViewModel>(entity));
    }


    /// <summary>
    /// 开启事务, 确保数据原子性
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    protected async ValueTask UnitOfWorkAsync(Func<IDbContextTransaction, ValueTask> action, CancellationToken cancellation = default)
    {
        await using var transaction = await DbContext.Database.BeginTransactionAsync(cancellation);

        try
        {
            await action(transaction);
            await transaction.CommitAsync(cancellation);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellation);
            throw;
        }
    }

    /// <summary>
    /// 开启事务, 确保数据原子性
    /// </summary>
    /// <param name="action"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    protected async ValueTask<TResult> UnitOfWorkAsync<TResult>(Func<IDbContextTransaction, ValueTask<TResult>> action, CancellationToken cancellation = default)
    {
        await using var transaction = await DbContext.Database.BeginTransactionAsync(cancellation);

        try
        {
            var result = await action(transaction);
            await transaction.CommitAsync(cancellation);

            return result;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellation);
            throw;
        }
    }
}



///// <summary>
/////     仓库
///// </summary>
///// <typeparam name="TDto"></typeparam>
//public interface Repository<TDto>
//    where TDto : IIdentifiable
//{
//    /// <summary>
//    ///     更新
//    /// </summary>
//    /// <param name="dto"></param>
//    /// <param name="cancellation"></param>
//    /// <returns></returns>
//    ValueTask<bool> UpdateAsync(TDto dto, CancellationToken cancellation = default);

//    /// <summary>
//    ///     添加
//    /// </summary>
//    /// <param name="dto"></param>
//    /// <param name="cancellation"></param>
//    /// <returns></returns>
//    ValueTask<bool> AddAsync(TDto dto, CancellationToken cancellation = default);

//    /// <summary>
//    ///     删除
//    /// </summary>
//    /// <param name="token"></param>
//    /// <param name="cancellation"></param>
//    /// <returns></returns>
//    ValueTask<bool> DeleteAsync(IIdentifiable token, CancellationToken cancellation = default);

//    /// <summary>
//    ///     查询
//    /// </summary>
//    /// <param name="token"></param>
//    /// <param name="cancellation"></param>
//    /// <returns></returns>
//    ValueTask<TDto?> FindByIdAsync(IIdentifiable token, CancellationToken cancellation = default);

//    /// <summary>
//    ///     获取所有
//    /// </summary>
//    /// <returns></returns>
//    ValueTask<IEnumerable<TDto>> GetAllAsync(CancellationToken cancellation = default);

//    /// <summary>
//    ///     获取所有
//    /// </summary>
//    /// <returns></returns>
//    ValueTask<IEnumerable<TDto>> GetAllAsync(int skip, int count, CancellationToken cancellation = default);
//}

//public abstract class RepositoryUpdaterExtensions<TDbContext, TEntity, TDto>(IServiceProvider services)
//    : Repository<TDto>, IInfrastructure
//    where TDbContext : DbContext
//    where TEntity : EntityBase
//    where TDto : IIdentifiable
//{
//    protected abstract IQueryable<TEntity> Queryable { get; }
//    protected TDbContext DbContext => services.GetRequiredService<TDbContext>();
//    protected IMapper Mapper => services.GetRequiredService<IMapper>();
//    protected ILogger Logger => services.GetRequiredService<ILogger<RepositoryUpdaterExtensions<TDbContext, TEntity, TDto>>>();

//    public IServiceProvider ServiceProvider => services;


//    public async ValueTask<bool> AddAsync(TDto dto, CancellationToken cancellation = default)
//    {
//        return await DbContext.UnitOfWorkAsync(async x =>
//        {
//            var entity = Mapper.Map<TEntity>(dto);

//            await DbContext.AddAsync(entity, cancellation);
//            var result = await DbContext.SaveChangesAsync(cancellation);

//            return result > 0;
//        }, ex => Logger.LogError(ex, "添加数据时发生错误"));
//    }

//    public async ValueTask<bool> UpdateAsync(TDto dto, CancellationToken cancellation = default)
//    {
//        return await DbContext.UnitOfWorkAsync(async x =>
//        {
//            var entity = await Queryable.FirstOrDefaultAsync(e => e.Id == dto.Id, cancellation);

//            if (entity is null)
//            {
//                Logger.LogWarning("更新失败, 未查询到实体: {dto}", dto);
//                return false;
//            }

//            Mapper.Map(dto, entity);
//            var result = await DbContext.SaveChangesAsync(cancellation);

//            return result > 0;
//        }, ex => Logger.LogError(ex, "更新数据时发生错误"));
//    }

//    public async ValueTask<bool> DeleteAsync(IIdentifiable token, CancellationToken cancellation = default)
//    {
//        return await DbContext.UnitOfWorkAsync(async x =>
//        {
//            var entity = await Queryable.FirstOrDefaultAsync(e => e.Id == token.Id, cancellation);
//            if (entity is null)
//            {
//                Logger.LogWarning("数据删除失败, 无符合Id");
//                return false;
//            }

//            DbContext.Remove(entity);
//            await DbContext.SaveChangesAsync(cancellation);

//            return true;
//        }, ex => Logger.LogError(ex, "删除数据时发生错误"));
//    }

//    public async ValueTask<TDto?> FindByIdAsync(IIdentifiable token, CancellationToken cancellation = default)
//    {
//        return await DbContext.UnitOfWorkAsync(async x =>
//        {
//            var entity = await Queryable.FirstOrDefaultAsync(e => e.Id == token.Id, cancellation);

//            if (entity is null)
//            {
//                Logger.LogWarning("查询数据失败, 未符合条件的Id: {id}", token.Id);
//                return default;
//            }

//            return EntityToDto(entity);
//        }, ex => Logger.LogError(ex, "查询数据时发生错误"));
//    }

//    public async ValueTask<IEnumerable<TDto>> GetAllAsync(CancellationToken cancellation = default)
//    {
//        return await DbContext.UnitOfWorkAsync(async x =>
//        {
//            var entityList = await Queryable.ToListAsync(cancellation);
//            return EntityToDto(entityList).ToArray();
//        }, ex => Logger.LogError(ex, "获取数据时发生错误")) ?? [];
//    }

//    public async ValueTask<IEnumerable<TDto>> GetAllAsync(int skip, int count, CancellationToken cancellation = default)
//    {
//        return await DbContext.UnitOfWorkAsync(async x =>
//        {
//            var entityList = await Queryable.Skip(skip).Take(count).ToListAsync(cancellation);
//            return EntityToDto(entityList).ToArray();
//        }, ex => Logger.LogError(ex, "获取数据时发生错误")) ?? [];
//    }


//    protected virtual TDto EntityToDto(TEntity entity)
//    {
//        var dto = Mapper.Map<TDto>(entity);
//        return dto;
//    }

//    protected virtual IEnumerable<TDto> EntityToDto(IEnumerable<TEntity> entitys)
//    {
//        foreach (var i in entitys) yield return EntityToDto(i);
//    }
//}