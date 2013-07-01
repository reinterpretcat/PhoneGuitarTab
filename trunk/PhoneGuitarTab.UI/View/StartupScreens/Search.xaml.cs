using Microsoft.Phone.Controls;
using PhoneGuitarTab.UI.Infrastructure.Enums;
using System.Windows.Controls;
using System.Windows.Data;

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
    }
}
