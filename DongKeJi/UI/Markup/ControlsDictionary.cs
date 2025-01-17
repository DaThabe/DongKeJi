using System.Windows;

namespace DongKeJi.UI.Markup;

public class ControlsDictionary : ResourceDictionary
{
    private const string DictionaryUri = "pack://application:,,,/DongKeJi;component/UI/Resource/DongKeJi.UI.xaml";

    public ControlsDictionary()
    {
        Source = new Uri(DictionaryUri, UriKind.Absolute);
    }
}