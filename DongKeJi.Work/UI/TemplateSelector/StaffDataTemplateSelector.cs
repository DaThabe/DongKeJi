using System.Windows;
using System.Windows.Controls;

namespace DongKeJi.Work.UI.TemplateSelector;

/// <summary>
///     员工模板选择器
/// </summary>
internal class StaffDataTemplateSelector : DataTemplateSelector
{
    /// <summary>
    ///     设计
    /// </summary>
    public required DataTemplate Designer { get; set; }

    /// <summary>
    ///     销售
    /// </summary>
    public required DataTemplate Saleperson { get; set; }


    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        //if (item is DesignerViewModel)
        //{
        //    return Designer;
        //}

        //if (item is SalespersonViewModel)
        //{
        //    return Salesperson;
        //}

        return base.SelectTemplate(item, container);
    }
}