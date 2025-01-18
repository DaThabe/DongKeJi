using System.IO;
using DongKeJi.Database;
using DongKeJi.WebView.Model.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DongKeJi.WebView.Model;

internal class WebViewDbContext(string dbFolder) : LocalDbContext(dbFolder, "Web")
{
    /// <summary>
    ///     网页
    /// </summary>
    public DbSet<PageEntity> Page { get; set; }

    public WebViewDbContext(IApplication applicationContext) : this(applicationContext.DirectoryDatabase)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(WebViewDbContext).Assembly);
    }
}


internal class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<WebViewDbContext>
{
    public WebViewDbContext CreateDbContext(string[] args)
    {
        return new WebViewDbContext(Path.Combine(AppContext.BaseDirectory, "Database"));
    }
}