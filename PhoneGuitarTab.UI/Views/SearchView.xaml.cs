using System.Windows.Controls;
using System.Windows.Data;
using PhoneGuitarTab.Core.Views;
using PhoneGuitarTab.UI.ViewModels;

namespace PhoneGuitarTab.UI.Views
{
    public partial class SearchView : ViewPage
    {
        public SearchView()
        {
            InitializeComponent();

            var viewModel = DataContext as SearchViewModel;
            viewModel.PropertyChanged += viewModel_PropertyChanged;
        }

        private void viewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedPage")
            {
                tabsFadeIn.Begin();
            }
        }

        private void OnSearchTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            // Update the binding source
            BindingExpression bindingExpr = textBox.GetBindingExpression(TextBox.TextProperty);
            bindingExpr.UpdateSource();
            var viewModel = DataContext as SearchViewModel;
            viewModel.CurrentTypedText = textBox.Text;
        }
    }
}