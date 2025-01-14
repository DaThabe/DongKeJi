using DongKeJi.Config;
using DongKeJi.Core.Model.Entity;
using DongKeJi.Database;
using DongKeJi.Entity;
using Microsoft.EntityFrameworkCore;

namespace DongKeJi.Core.Model;


internal class CoreDbContext(IApplication applicationContext) : LocalDbContext(applicationContext, "Core"), IConfigDbContext
{
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<ConfigEntity> Configs { get; set; }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(CoreDbContext).Assembly);
    }
}