
namespace PhoneGuitarTab.UI.Notation
{
    public partial class SearchForBandView : PhoneGuitarTab.Core.ViewPage
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