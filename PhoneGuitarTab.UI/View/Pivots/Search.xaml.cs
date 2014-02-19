using Microsoft.Phone.Controls;
using System.Windows.Controls;
using System.Windows.Data;

namespace PhoneGuitarTab.UI.Pivots
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
