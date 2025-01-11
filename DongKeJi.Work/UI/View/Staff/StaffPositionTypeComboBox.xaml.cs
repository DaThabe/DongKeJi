using System.Windows.Controls;
using DongKeJi.Work.Model.Entity.Staff;

namespace DongKeJi.Work.UI.View.Staff;

/// <summary>
///     OrderStateSelectorViewModel.xaml 的交互逻辑
/// </summary>
public partial class StaffPositionTypeComboBox : UserControl
{
    public StaffPositionTypeComboBox()
    {
        InitializeComponent();

        ComboBox.ItemsSource = new[]
        {
            StaffPositionType.Designer,
            StaffPositionType.Salesperson
        };
    }
}