using Microsoft.Extensions.Hosting;

namespace DongKeJi.Common.Module;

public interface IModule
{
    public string Title { get; }

    public string Describe { get; }


    void Configure(IHostBuilder builder);
}