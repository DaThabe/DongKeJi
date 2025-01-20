using DongKeJi.Inject;
using DongKeJi.Module;
using DongKeJi.ViewModel;
using DongKeJi.Web.Model;
using DongKeJi.Web.Service;
using DongKeJi.Web.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DongKeJi.Web;


/// <summary>
/// 办公模块
/// </summary>
public interface IWebModule : IModule
{
    /// <summary>
    /// 当前页面
    /// </summary>
    PageViewModel? CurrentPage { get; internal set; }
}

/// <summary>
/// 网页视图模块
/// </summary>
[Inject(ServiceLifetime.Singleton, typeof(IWebModule))]
public class WebModule : ObservableViewModel, IWebModule
{
    public static IModuleInfo Info { get; } = ModuleInfo.CreateFromAssembly<WebModule>(config =>
    {
        config.Name = "DongKeJi.Web";
        config.Title = "网页";
        config.Developers = ["DaThabe"];
        config.CreatedAt = new DateTime(2025, 1, 18);
        config.ReleaseDate = new DateTime(2025, 1, 18);
    });

    public static void Configure(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            //反射注入
            services.AddAutoInject<WebModule>();
            //启动后业务
            services.AddHostedService<HostedService>();
            
            //数据库
            services.AddDbContext<WebViewDbContext>();
            //AutoMapper
            services.AddAutoMapper(typeof(WebViewMapperProfile));
        });
    }

    public PageViewModel? CurrentPage { get; set; }
}