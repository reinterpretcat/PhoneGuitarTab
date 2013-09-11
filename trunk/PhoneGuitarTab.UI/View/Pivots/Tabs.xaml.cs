using Microsoft.Phone.Controls;

namespace PhoneGuitarTab.UI.Pivots
{
    public partial class Tabs : PivotItem
    {
        public Tabs()
        {
            InitializeComponent();
        }

        private void ToTopButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.TabList.ScrollTo(this.TabList.Tag);
        }
    }
}
