using AutoMapper;
using DongKeJi.Database;
using DongKeJi.Entity;
using DongKeJi.Inject;
using DongKeJi.ViewModel;
using DongKeJi.Web.Model;
using DongKeJi.Web.Model.Entity;
using DongKeJi.Web.ViewModel;
using Microsoft.Extensions.DependencyInjection;

namespace DongKeJi.Web;

/// <summary>
/// 办公模块数据库
/// </summary>
public interface IWebDatabase
{
    internal IServiceProvider ServiceProvider { get; }
}

[Inject(ServiceLifetime.Singleton, typeof(IWebDatabase))]
internal class WeDatabase(IServiceProvider services) : IWebDatabase
{
    public IServiceProvider ServiceProvider { get; } = services;
}

public static class WebDatabaseExtensions
{
    private static IAutoUpdateBuilder<TViewModel> AutoUpdate<TEntity, TViewModel>(this IWebDatabase db, TViewModel viewModel)
        where TEntity : EntityBase
        where TViewModel : IEntityViewModel
    {
        var mapper = db.ServiceProvider.GetRequiredService<IMapper>();
        var dbContext = db.ServiceProvider.GetRequiredService<WebViewDbContext>();

        return dbContext.AutoUpdate<TEntity, TViewModel>(viewModel, mapper);
    }

    public static IAutoUpdateBuilder<PageViewModel> AutoUpdate(this IWebDatabase db, PageViewModel vm)
        => db.AutoUpdate<PageEntity, PageViewModel>(vm);
}