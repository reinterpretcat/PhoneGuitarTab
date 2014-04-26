using Microsoft.Phone.Controls;
using PhoneGuitarTab.UI.Entities;
using PhoneGuitarTab.UI.ViewModel;
using PhoneGuitarTab.Controls;
using PhoneGuitarTab.UI.View;
namespace PhoneGuitarTab.UI.PanaromaItems
{
    public partial class Tabs : PanoramaItem
    {
        public Tabs()
        {
            InitializeComponent();
           
        }

       

        private void TabList_IsSelectionEnabledChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            var viewModel = DataContext as CollectionViewModel;
            viewModel.SetIsSelectionEnabled.Execute(sender);
        }
       
    }
}
