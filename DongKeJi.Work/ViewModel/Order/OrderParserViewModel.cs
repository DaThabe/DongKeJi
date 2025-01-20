using System.ComponentModel.DataAnnotations;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DongKeJi.ViewModel;
using DongKeJi.Work.Model.Entity.Order;
using System.Text.RegularExpressions;
using System.Windows;
using DongKeJi.Inject;
using DongKeJi.UI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Wpf.Ui;

namespace DongKeJi.Work.ViewModel.Order;


[Inject(ServiceLifetime.Transient)]
public partial class OrderParserViewModel(
    ILogger<OrderParserViewModel> logger,
    ISnackbarService snackbarService
    ) : ObservableViewModel
{
    [ObservableProperty] 
    private string? _orderInfoText;

    [ObservableProperty]
    private TextOrderInfoValue? _orderInfoValue;


    [RelayCommand]
    private void PasteOrderText()
    {
        try
        {
            var orderText = Clipboard.GetText();
            OrderInfoText = orderText;
            CreateOrderByText(orderText);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "粘贴订单文本时发生错误");
            snackbarService.ShowError(ex, x => x.Title = "粘贴订单文本时发生错误");
        }
    }

    [RelayCommand]
    private void CreateOrderByText(string text)
    {
        Dictionary<string, string> info = [];
        StringBuilder errorInfoList = new();

        try
        {
            info = GetOrderInfo(text);

            OrderInfoValue = new TextOrderInfoValue()
            {
                CustomerName = GetCustomerName(),
                OrderPrice = GetPrice(),
                OrderState = GetOrderState(),
                SalespersonName = GetSalespersonName(),
                OrderCreateTime = GetCreateTime()
            };

            var errorString = errorInfoList.ToString();
            if (!string.IsNullOrWhiteSpace(errorString))
            {
                throw new ValidationException(errorString);
            }
        }
        catch (Exception ex)
        {
            OrderInfoValue = null;
            logger.LogError(ex, "根据订单文本创建订单时发生错误");
            snackbarService.ShowError(ex, x => x.Title = "根据订单文本创建订单时发生错误");
        }

        return;

        DateTime GetCreateTime()
        {
            try
            {
                var dateText = info["合作时间"];
                return Convert.ToDateTime(dateText);
            }
            catch(Exception ex)
            {
                errorInfoList.AppendLine($"合作时间解析失败: {ex.Message}");
                return DateTime.Now;
            }
        }

        string? GetSalespersonName()
        {
            try
            {
                return info["销售"];
            }
            catch (Exception ex)
            {
                errorInfoList.AppendLine($"销售名称解析失败: {ex.Message}");
                return null;
            }
        }

        string? GetCustomerName()
        {
            try
            {
                return info["机构"];
            }
            catch(Exception ex)
            {
                errorInfoList.AppendLine($"机构名称解析失败: {ex.Message}");
                return null;
            }
        }

        OrderState GetOrderState()
        {
            try
            {
                var stateText = info["到账"];

                if ("已到账".Contains(stateText))
                {
                    return OrderState.Active;
                }

                if ("未到账".Contains(text))
                {
                    return OrderState.Ready;
                }
            }
            catch(Exception ex)
            {
                errorInfoList.AppendLine($"订单状态解析失败: {ex.Message}");
            }
            
            return OrderState.None;
        }

        double GetPrice()
        {
            try
            {
                var priceText = Regex.Match(info["价格"], @"\d+").Value;
                return double.Parse(priceText);
            }
            catch (Exception ex)
            {
                errorInfoList.AppendLine($"订单价格解析失败: {ex.Message}");
                return 0;
            }
        }
    }


    private Dictionary<string, string> GetOrderInfo(string text)
    {
        Dictionary<string, string> values = [];
        var lines = text.Split('\n').Where(x => !string.IsNullOrWhiteSpace(x));

        foreach (var line in lines)
        {
            var splitIndex = line.IndexOfAny(['：', ':']);

            var key = line[..splitIndex].Trim();
            var value = line[(splitIndex + 1)..].Trim();

            values[key] = value;
        }

        return values;
    }
}


public class TextOrderInfoValue
{
    public required string? CustomerName { get; set; }
    public required double OrderPrice { get; set; }
    public required OrderState OrderState { get; set; }
    public required string? SalespersonName { get; set; }
    public required DateTime OrderCreateTime { get; set; }
}