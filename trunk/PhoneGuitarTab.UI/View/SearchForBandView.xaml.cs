
namespace PhoneGuitarTab.UI.Notation
{
    using PhoneGuitarTab.Core.Views;

    public partial class SearchForBandView : ViewPage
    {
        public SearchForBandView()
        {
            InitializeComponent();
        }

        private void ToTopButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.tabsList.ScrollTo(this.tabsList.Tag);
        }
    }
}