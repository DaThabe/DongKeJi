using DongKeJi.Core.Model.Entity;
using DongKeJi.Database;
using Microsoft.EntityFrameworkCore;

namespace DongKeJi.Core.Model;


internal class CoreDbContext(IApplication application) : LocalDbContext(application, "Core")
{
    public DbSet<UserEntity> Users { get; set; }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(CoreDbContext).Assembly);
    }
}