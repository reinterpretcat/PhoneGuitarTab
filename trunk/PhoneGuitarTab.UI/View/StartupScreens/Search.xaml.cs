using Microsoft.Phone.Controls;
using PhoneGuitarTab.UI.Infrastructure.Enums;
using System.Windows.Controls;
using System.Windows.Data;
using System.Linq;
using System.Collections;
using PhoneGuitarTab.UI.Entities;

namespace PhoneGuitarTab.UI.View
{
    public partial class Search : PivotItem
    {
        public Search()
        {
            InitializeComponent();
        }

        private void OnSearchTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            // Update the binding source
            BindingExpression bindingExpr = textBox.GetBindingExpression(TextBox.TextProperty);
            bindingExpr.UpdateSource();
        }

        private void ToTopButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //var groupList = this.tabsList.ItemsSource;
            //if (groupList != null && (groupList as IList) != null)
            //    this.tabsList.ScrollToGroup(((IList)groupList).);

            this.tabsList.ScrollTo(this.tabsList.Tag);
        }
    }
}
