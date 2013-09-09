using Microsoft.Phone.Controls;
using System;
using System.Windows.Controls;

namespace PhoneGuitarTab.UI.View
{
    public partial class GroupView : PhoneGuitarTab.Core.ViewPage
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