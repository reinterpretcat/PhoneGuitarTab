using PhoneGuitarTab.Core.Views;

namespace PhoneGuitarTab.UI.Views
{
    public partial class SearchForBandView : ViewPage
    {
        public SearchForBandView()
        {
            InitializeComponent();
        }

        private void ToTopButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            tabsList.ScrollTo(tabsList.Tag);
        }
    }
}