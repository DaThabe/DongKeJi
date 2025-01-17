using System.IO;
using DongKeJi.Config;
using DongKeJi.Core.Model.Entity;
using DongKeJi.Database;
using DongKeJi.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DongKeJi.Core.Model;


internal class CoreDbContext(string dbFolder) : LocalDbContext(dbFolder, "Core"), IConfigDbContext
{
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<ConfigEntity> Config { get; set; }

    public CoreDbContext(IApplication application) : this(application.DatabaseDirectory)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(CoreDbContext).Assembly);
    }
}


internal class DesignTimeWorkRecordDbContextFactory : IDesignTimeDbContextFactory<CoreDbContext>
{
    public CoreDbContext CreateDbContext(string[] args)
    {
        return new CoreDbContext(Path.Combine(AppContext.BaseDirectory, "Database"));
    }
}