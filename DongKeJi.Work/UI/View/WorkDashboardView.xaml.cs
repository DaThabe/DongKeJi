
using System.Windows.Controls;
using DongKeJi.Extensions;
using DongKeJi.Inject;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using LiveCharts;
using Microsoft.Extensions.DependencyInjection;
using Separator = LiveCharts.Wpf.Separator;

namespace DongKeJi.Work.UI.View;


[Inject(ServiceLifetime.Singleton)]
public partial class WorkDashboardView : UserControl
{
    public WorkDashboardView()
    {
        InitializeComponent();

        var date = new DateTime(2025, 1, 15);
        List<MonthlyHeatmapItem> list = [];

        for (var i = 1; i <= 31; i++)
        {
            if (Random.Shared.NextBoolean(0.1)) continue;

            list.Add(new MonthlyHeatmapItem(i, Random.Shared.Next(1, 15)));
        }
        var chart = CreateMonthlyHeatmap(date, list);

        // 使用 HeatSeries 创建热力图
        HeatmapChart.AxisX.AddRange(chart.AxisX);
        HeatmapChart.AxisY.AddRange(chart.AxisY);
        HeatmapChart.Series.AddRange(chart.Series);
    }

    public record MonthlyHeatmapItem(int Day, double Value);

    public static CartesianChart CreateMonthlyHeatmap(DateTime date, IEnumerable<MonthlyHeatmapItem> values)
    {
        return new CartesianChart
        {
            AxisX =
            [
                new Axis
                {
                    Position = AxisPosition.RightTop,
                    Labels = ["周一", "周二", "周三", "周四", "周五", "周六", "周日"],
                    Separator = new Separator { Step = 1, IsEnabled = false }
                }
            ],
            AxisY =
            [
            ],
            Series =
            [
                new HeatSeries
                {
                    Title = "当日消耗",
                    Values = CreateHeatmap(values, date),
                    DataLabels = true
                }
            ]
        };
    }

    public static ChartValues<HeatPoint> CreateHeatmap(IEnumerable<MonthlyHeatmapItem> values, DateTime date)
    {
        const int columns = 7; // 每行 7 列（周一到周日）

        var daysInMonth = DateTime.DaysInMonth(date.Year, date.Month);
        var offset = (int)new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).DayOfWeek - 1; // 计算 1 号的偏移量（周一为 0）
        if (offset < 0) offset += 7; // 确保偏移量非负

        var totalCells = daysInMonth + offset; // 包含偏移的总单元格数
        var rows = (totalCells + columns - 1) / columns; // 计算行数

        var chartValues = new ChartValues<HeatPoint>();

        // 遍历数据，按照偏移量计算正确的行列
        foreach (var item in values)
        {
            var index = item.Day - 1 + offset; // 偏移后的索引
            var row = index / columns;        // 计算行号
            var column = index % columns;    // 计算列号

            // 添加到热力图值
            chartValues.Add(new HeatPoint(column, rows - row, item.Value));
        }

        return chartValues;
    }
}