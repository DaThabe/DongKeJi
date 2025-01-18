using DongKeJi.Inject;
using DongKeJi.Module;
using DongKeJi.ViewModel;
using DongKeJi.WebView.Model;
using DongKeJi.WebView.Service;
using DongKeJi.WebView.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DongKeJi.WebView;


/// <summary>
/// �칫ģ��
/// </summary>
public interface IWebViewModule : IModule
{
    /// <summary>
    /// ��ǰҳ��
    /// </summary>
    PageViewModel? CurrentPage { get; internal set; }
}

/// <summary>
/// ��ҳ��ͼģ��
/// </summary>
[Inject(ServiceLifetime.Singleton, typeof(IWebViewModule))]
public class WebViewModule : ObservableViewModel, IWebViewModule
{
    public static IModuleInfo Info { get; }= new ModuleInfo
    {
        Id = Guid.NewGuid(),
        Name = "DongKeJi.WebView",
        Version = new Version(0, 0, 1),
        Title = "��ҳ",
        Developers = ["DaThabe"],
        Describe = """
                   ### ��ҳģ��
                   - �ṩ��ҳ�ղغ��������
                   """,
        CreatedAt = new DateTime(2025, 1, 18),
        ReleaseDate = new DateTime(2025, 1, 18),
        Dependencies = typeof(WebViewModule).Assembly.GetReferencedAssemblies()
    };

    public static void Configure(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            //����ע��
            services.AddAutoInject<WebViewModule>();
            //������ҵ��
            services.AddHostedService<HostedService>();
            
            //���ݿ�
            services.AddDbContext<WebViewDbContext>();
            //AutoMapper
            services.AddAutoMapper(typeof(WebViewMapperProfile));
        });
    }

    public PageViewModel? CurrentPage { get; set; }
}