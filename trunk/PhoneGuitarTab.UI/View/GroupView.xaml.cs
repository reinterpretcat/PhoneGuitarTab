using Microsoft.Phone.Controls;
using System;
using System.Windows.Controls;

namespace PhoneGuitarTab.UI.View
{
    using PhoneGuitarTab.Core.Views;

    public partial class GroupView : ViewPage
    {
        public GroupView()
        {
            InitializeComponent();
        }

        public void PivotSelectionChangedHandler(object sender, SelectionChangedEventArgs e)
        {
            this.RefreshButton.IsEnabled = this.PivotControl.SelectedIndex == 1;
        }
    }
}