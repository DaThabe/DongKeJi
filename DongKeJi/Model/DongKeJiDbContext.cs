using DongKeJi.Common.Database;
using DongKeJi.Model.Entity;
using Microsoft.EntityFrameworkCore;

namespace DongKeJi.Model;

public class DongKeJiDbContext() : DatabaseContext("Core")
{
    public DbSet<UserEntity> Users { get; set; }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(DongKeJiDbContext).Assembly);
    }
}