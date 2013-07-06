
namespace PhoneGuitarTab.UI.Notation
{
    public partial class SearchView : PhoneGuitarTab.Core.ViewPage
    {
        public SearchView()
        {
            InitializeComponent();
        }

        private void ToTopButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.tabsList.ScrollTo(this.tabsList.Tag);
        }
    }
}