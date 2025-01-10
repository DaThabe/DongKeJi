﻿using DongKeJi.Common;
using DongKeJi.Common.Exceptions;
using DongKeJi.Common.Extensions;
using DongKeJi.Common.Inject;
using DongKeJi.Common.Service;
using DongKeJi.Work.Model;
using DongKeJi.Work.Model.Entity.Staff;
using DongKeJi.Work.ViewModel.Common.Staff;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using DongKeJi.Work.Model.Validation;
using DongKeJi.Common.Validation;

namespace DongKeJi.Work.Service;

public interface IStaffPositionService
{
    /// <summary>
    ///     绑定职位
    /// </summary>
    ValueTask<StaffPositionViewModel> BindingAsync(
        StaffPositionType positionType, 
        IIdentifiable staff,
        CancellationToken cancellation = default);

    /// <summary>
    ///     解除绑定职位
    /// </summary>
    ValueTask UnbindingAsync(
        StaffPositionType positionType, 
        IIdentifiable staff,
        CancellationToken cancellation = default);

    /// <summary>
    ///     解除绑定员工的所有职位
    /// </summary>
    ValueTask UnbindingAsync(
        IIdentifiable staff, 
        CancellationToken cancellation = default);

    /// <summary>
    ///     设置职位信息, 有则更新, 没有则创建 (根据职位类型<see cref="StaffPositionType" />判断是否存在
    /// </summary>
    /// <param name="position"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    ValueTask SetAsync(
        StaffPositionViewModel position, 
        CancellationToken cancellation = default);

    /// <summary>
    ///     删除职位信息
    /// </summary>
    /// <param name="positionType"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    ValueTask RemoveAsync(
        StaffPositionType positionType, 
        CancellationToken cancellation = default);

    /// <summary>
    ///     根据类型查询职位信息
    /// </summary>
    /// <param name="positionType"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    ValueTask<StaffPositionViewModel> FindByTypeAsync(
        StaffPositionType positionType,
        CancellationToken cancellation = default);

    /// <summary>
    ///     查询员工的所有职位
    /// </summary>
    /// <param name="staff"></param>
    /// <param name="take"></param>
    /// <param name="cancellation"></param>
    /// <param name="skip"></param>
    /// <returns></returns>
    ValueTask<IEnumerable<StaffPositionViewModel>> FindAllByStaffAsync(
        IIdentifiable staff, 
        int? skip = null,
        int? take = null, 
        CancellationToken cancellation = default);

    /// <summary>
    ///     获取所有职位
    /// </summary>
    /// <param name="take"></param>
    /// <param name="cancellation"></param>
    /// <param name="skip"></param>
    /// <returns></returns>
    ValueTask<IEnumerable<StaffPositionViewModel>> GetAllAsync(
        int? skip = null, 
        int? take = null,
        CancellationToken cancellation = default);
}

[Inject(ServiceLifetime.Transient, typeof(IStaffPositionService))]
internal class StaffPositionService(
    IServiceProvider services, 
    IMapper mapper,
    WorkDbContext dbContext) :  IStaffPositionService
{
    public async ValueTask<StaffPositionViewModel> BindingAsync(
        StaffPositionType positionType,
        IIdentifiable staff,
        CancellationToken cancellation = default)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellation);

        try
        {
            //验证
            positionType.AssertValidate();

            //员工
            var staffEntity = await dbContext.Staffs
                .Include(x => x.Positions)
                .FirstOrDefaultAsync(x => x.Id == staff.Id, cancellation);
            staffEntity = RepositoryException.ThrowIfEntityNotFound(staffEntity, "员工不存在");

            //职位
            var staffPositionEntity = await dbContext.StaffPositions
                .Include(x => x.Staffs)
                .FirstOrDefaultAsync(x => x.Type == positionType, cancellation);

            if (staffPositionEntity is null || staffPositionEntity.IsEmpty())
            {
                staffPositionEntity = new StaffPositionEntity
                {
                    Type = positionType,
                    Title = positionType.ToString()
                };
            }

            //职位-员工 多对多
            staffPositionEntity.Staffs.Add(staffEntity, x => x.Id != staffEntity.Id);
            staffEntity.Positions.Add(staffPositionEntity, x => x.Type != staffPositionEntity.Type);

            //保存
            await dbContext.AssertSaveSuccessAsync(cancellation: cancellation);
            await transaction.CommitAsync(cancellation);

            return dbContext.RegisterAutoUpdate<StaffPositionEntity, StaffPositionViewModel>(staffPositionEntity, services);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellation);
            throw new RepositoryException($"关联员工职位时发生错误\n职位类型: {positionType}\n员工Id: {staff.Id}", ex);
        }
    }

    public async ValueTask UnbindingAsync(
        StaffPositionType positionType,
        IIdentifiable staff,
        CancellationToken cancellation = default)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellation);

        try
        {
            var staffEntity = await dbContext.Staffs
                .Include(x => x.Positions)
                .FirstOrDefaultAsync(x => x.Id == staff.Id, cancellation);
            staffEntity = RepositoryException.ThrowIfEntityNotFound(staffEntity, "员工不存在");

            //保存
            staffEntity.Positions.Remove(x => x.Type == positionType);
            
            await dbContext.AssertSaveSuccessAsync(cancellation: cancellation);
            await transaction.CommitAsync(cancellation);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellation);
            throw new RepositoryException($"解除员工职位时发生错误\n职位类型: {positionType}\n员工Id: {staff.Id}", ex);
        }
    }

    public async ValueTask UnbindingAsync(
        IIdentifiable staff,
        CancellationToken cancellation = default)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellation);

        try
        {
            var staffEntity = await dbContext.Staffs
                .Include(x => x.Positions)
                .FirstOrDefaultAsync(x => x.Id == staff.Id, cancellation);
            staffEntity = RepositoryException.ThrowIfEntityNotFound(staffEntity, "员工不存在");

            staffEntity.Positions.Clear();

            await dbContext.AssertSaveSuccessAsync(cancellation: cancellation);
            await transaction.CommitAsync(cancellation);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellation);
            throw new RepositoryException($"解除员工所有职位时发生错误\n员工Id: {staff.Id}", ex);
        }
    }

    public async ValueTask SetAsync(
        StaffPositionViewModel position,
        CancellationToken cancellation = default)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellation);

        try
        {
            //验证
            validation.Validate(position);

            //职位
            var positionEntity = await dbContext.StaffPositions
                .FirstOrDefaultAsync(x => x.Type == position.Type, cancellation);

            if (positionEntity is null || positionEntity.IsEmpty())
            {
                //新增
                positionEntity = mapper.Map<StaffPositionEntity>(position);
                await dbContext.AddAsync(positionEntity, cancellation);
            }
            else
            {
                //更新
                mapper.Map(positionEntity, position);
            }

            //保存
            await dbContext.AssertSaveSuccessAsync(cancellation: cancellation);
            await transaction.CommitAsync(cancellation);

            dbContext.RegisterAutoUpdate<StaffPositionEntity, StaffPositionViewModel>(position, services);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellation);
            throw new RepositoryException($"设置员工职位时发生错误\n职位信息: {position}", ex);
        }
    }

    public async ValueTask RemoveAsync(
        StaffPositionType positionType,
        CancellationToken cancellation = default)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellation);

        try
        {
            var positionEntity = await dbContext.StaffPositions
                .FirstOrDefaultAsync(x => x.Type == positionType, cancellation);
            positionEntity = RepositoryException.ThrowIfEntityNotFound(positionEntity, "职位不存在");

            dbContext.Remove(positionEntity);

            await dbContext.AssertSaveSuccessAsync(cancellation: cancellation);
            await transaction.CommitAsync(cancellation);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellation);
            throw new RepositoryException($"删除职位时发生错误\n职位类型: {positionType}", ex);
        }
    }

    public async ValueTask<StaffPositionViewModel> FindByTypeAsync(
        StaffPositionType positionType,
        CancellationToken cancellation = default)
    {
        //await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellation);

        try
        {
            var positionEntity = await dbContext.StaffPositions
                .FirstOrDefaultAsync(x => x.Type == positionType, cancellation);
            positionEntity = RepositoryException.ThrowIfEntityNotFound(positionEntity, "职位不存在");

            return dbContext.RegisterAutoUpdate<StaffPositionEntity, StaffPositionViewModel>(positionEntity, services);
        }
        catch (Exception ex)
        {
            //await transaction.RollbackAsync(cancellation);
            throw new RepositoryException($"删除职位时发生错误\n职位类型: {positionType}", ex);
        }
    }

    public async ValueTask<IEnumerable<StaffPositionViewModel>> FindAllByStaffAsync(
        IIdentifiable staff,
        int? skip = null,
        int? take = null,
        CancellationToken cancellation = default)
    {
        //await using var transaction = await DbContext.Database.BeginTransactionAsync(cancellation);

        try
        {
            var positionEntityList = await dbContext.Staffs
                .Include(x => x.Positions)
                .Where(x => x.Id == staff.Id)
                .Select(x => x.Positions.SkipAndTake(skip, take).ToList())
                .FirstOrDefaultAsync(cancellation) ?? [];

            return positionEntityList.Select(x => dbContext.RegisterAutoUpdate<StaffPositionEntity, StaffPositionViewModel>(x, services));
        }
        catch (Exception ex)
        {
            //await transaction.RollbackAsync(cancellation);
            throw new RepositoryException($"获取员工下所有职位时发生错误\n员工Id: {staff.Id}", ex);
        }
    }

    public async ValueTask<IEnumerable<StaffPositionViewModel>> GetAllAsync(
        int? skip = null,
        int? take = null,
        CancellationToken cancellation = default)
    {
        try
        {
            var positionEntityList = await dbContext.StaffPositions
                .SkipAndTake(skip, take)
                .ToListAsync(cancellation);

            return positionEntityList.Select(x => dbContext.RegisterAutoUpdate<StaffPositionEntity, StaffPositionViewModel>(x, services));
        }
        catch (Exception ex)
        {
            //await transaction.RollbackAsync(cancellation);
            throw new RepositoryException("获取所有职位时发生错误", ex);
        }
    }
}