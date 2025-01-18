using System.ComponentModel.DataAnnotations.Schema;
using DongKeJi.Entity;

namespace DongKeJi.WebView.Model.Entity;


[Table("PageItem")]
internal class PageEntity : EntityBase, IPage
{
    /// <summary>
    /// 标题
    /// </summary>
    [Column("Title")]
    public required string Title { get; set; }

    /// <summary>
    /// 页面网址
    /// </summary>
    [Column("Source")]
    public required Uri Source { get; set; }
}