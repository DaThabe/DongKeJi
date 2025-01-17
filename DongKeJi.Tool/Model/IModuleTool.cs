using DongKeJi.Module;

namespace DongKeJi.Tool.Model;

public interface IModuleTool
{
    /// <summary>
    /// 模块信息
    /// </summary>
    IModuleInfo ModuleInfo { get; }

    /// <summary>
    /// 所有工具
    /// </summary>
    IEnumerable<IToolItem> ToolItems { get; }
}


public class ModuleTool(IModuleInfo iModuleInfo, IEnumerable<IToolItem> toolItems) : IModuleTool
{
    public IModuleInfo ModuleInfo => iModuleInfo;

    public IEnumerable<IToolItem> ToolItems => toolItems;
}