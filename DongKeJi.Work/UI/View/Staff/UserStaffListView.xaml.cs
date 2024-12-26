using System.Windows.Controls;
using Wpf.Ui.Controls;
using CustomerViewModel = DongKeJi.Work.ViewModel.Customer.CustomerViewModel;

namespace DongKeJi.Work.UI.View.Staff;

/// <summary>
///     CustomerListView.xaml 的交互逻辑
/// </summary>
public partial class UserStaffListView : UserControl
{
    public UserStaffListView()
    {
        InitializeComponent();
    }

    private void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
    {
        if (DataContext is not CustomerServiceViewModel customerList) return;

        if (args.SelectedItem is CustomerViewModel customer) customerList.Selected = customer;
    }
}