using System;
using PhoneGuitarTab.UI.ViewModel;
using Microsoft.Phone.Controls;


namespace PhoneGuitarTab.UI
{
    public partial class TextTabView : Core.ViewPage
    {
        public TextTabView()
        {
            InitializeComponent();
            tabWebBrowser.Loaded += delegate
                {
                    var viewModel = DataContext as TextTabViewModel;
                    if (viewModel.TabContent != null)
                        tabWebBrowser.NavigateToString(viewModel.TabContent);
                };
        }

        private void tabWebBrowser_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            int i = 1;
            i = i++;
        }
    }
}