using Microsoft.Phone.Controls;
using PhoneGuitarTab.UI.Entities;
using PhoneGuitarTab.UI.ViewModel;
using PhoneGuitarTab.Controls;
using PhoneGuitarTab.UI.View;
namespace PhoneGuitarTab.UI.Pivots
{
    public partial class Tabs : PivotItem
    {
        public Tabs()
        {
            InitializeComponent();
            
        }
       

        //Unfortunately, not possible to trigger this event via ViewModel
        private void TabList_IsSelectionEnabledChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {  
            var viewModel = DataContext as CollectionViewModel;
            viewModel.SetIsSelectionEnabled.Execute(sender);       
        }

       

    }
}
