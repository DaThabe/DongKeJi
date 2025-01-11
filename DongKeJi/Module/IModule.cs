﻿using Microsoft.Extensions.Hosting;

namespace DongKeJi.Module;

public interface IModule
{
    /// <summary>
    /// 元信息
    /// </summary>
    IModuleMetaInfo MetaInfo { get; }

    /// <summary>
    /// 配置模块
    /// </summary>
    /// <param name="builder"></param>
    void Configure(IHostBuilder builder);
}