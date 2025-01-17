using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using DongKeJi.Inject;
using DongKeJi.UI;
using Microsoft.Extensions.DependencyInjection;
using Wpf.Ui;
using Wpf.Ui.Appearance;
using Button = System.Windows.Controls.Button;
using TextBox = System.Windows.Controls.TextBox;

namespace DongKeJi.Launcher.UI.View.Color;


[Inject(ServiceLifetime.Singleton)]
public partial class ColorView 
{
    // 注册依赖属性
    public static readonly DependencyProperty SystemColorsProperty = DependencyProperty.Register(
            nameof(SystemColors),
            typeof(ColorItemList),
            typeof(ColorView),
            new PropertyMetadata(new ColorItemList()));

    // 注册依赖属性
    public static readonly DependencyProperty SelectedSystemColorProperty = DependencyProperty.Register(
        nameof(SelectedSystemColor),
        typeof(ColorItem),
        typeof(ColorView));

    // CLR包装器
    public ColorItemList SystemColors
    {
        get => (ColorItemList)GetValue(SystemColorsProperty);
        set => SetValue(SystemColorsProperty, value);
    }
    
    // CLR包装器
    public ColorItem? SelectedSystemColor
    {
        get => (ColorItem)GetValue(SelectedSystemColorProperty);
        set
        {
            SetValue(SelectedSystemColorProperty, value);

            var text = value?.Key.ToString();
            if(string.IsNullOrWhiteSpace(text)) return;
            Clipboard.SetText(text);

            _snackbarService.ShowInfo($"已复制到剪切板: {text}");
        }
    }



    public ColorView(IApplication application, ISnackbarService snackbarService)
    {
        _application = application;
        _snackbarService = snackbarService;
        InitializeComponent();
    }


    protected override async ValueTask OnNavigatedToAsync(CancellationToken cancellation = default)
    {
        ColorItemList colors = [];
        GetAllResources(ref colors, _application.Resources);
        _allColorItemList = colors;

        SystemColors = colors;

        await base.OnNavigatedToAsync(cancellation);
    }

    protected override ValueTask OnNavigatedFromAsync(CancellationToken cancellation = default)
    {
        SystemColors.Clear();
        return ValueTask.CompletedTask;
    }

    private void GetAllResources(ref ColorItemList list, ResourceDictionary resourceDictionary)
    {
        // 遍历当前字典的资源
        foreach (var key in resourceDictionary.Keys)
        {
            try
            {
                if (SystemColors.Find(x => x.Key == key) != null) continue;

                var value = resourceDictionary[key];
                if (value is not Brush brush) continue;

                list.Add(new ColorItem { Key = key, Brush = brush });
            }
            catch
            {
                // ignored
            }
        }

        // 遍历合并字典的资源
        foreach (var mergedDictionary in resourceDictionary.MergedDictionaries)
        {
            GetAllResources(ref list, mergedDictionary);
        }
    }

    private void ThemeButtonClick(object sender, RoutedEventArgs e)
    {
        if (_application.Theme == ApplicationTheme.Light)
        {
            _application.Theme = ApplicationTheme.Dark;
        }
        else
        {
            _application.Theme = ApplicationTheme.Light;
        }
    }



    private readonly IApplication _application;
    private readonly ISnackbarService _snackbarService;

    private void CopyColorClick(object sender, RoutedEventArgs e)
    {
        if (sender is not Button btn) return;
        var name = btn.Tag.ToString();
        if (string.IsNullOrWhiteSpace(name)) return;

        Clipboard.SetText(name);
        _snackbarService.ShowInfo($"已复制到剪切板: {name}");
    }


    private void InputKeyDown(object sender, KeyEventArgs e)
    {
        if (sender is not TextBox tb) return;
        if (e.Key != Key.Enter) return;

        if (string.IsNullOrWhiteSpace(tb.Text))
        {
            SystemColors = _allColorItemList;
        }

        var results = _allColorItemList.Where(x => x.Key.ToString()?.Contains(tb.Text, StringComparison.CurrentCultureIgnoreCase) == true);
        SystemColors = [.. results];
    }

    private ColorItemList _allColorItemList = [];
}


public class ColorItem
{
    public required object Key { get; init; }
    public required Brush Brush { get; init; }

    public override string ToString()
    {
        return Key.ToString() ?? "";
    }
}

public class ColorItemList : List<ColorItem>;