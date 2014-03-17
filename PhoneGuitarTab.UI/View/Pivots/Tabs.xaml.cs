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


        private void TabList_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var selector = sender as LongListMultiSelector;
            if (!selector.IsSelectionEnabled)
            {
                var itemTapped = (System.Windows.FrameworkElement)e.OriginalSource;
                var tab = (itemTapped.DataContext) as TabEntity;
                var viewModel = DataContext as CollectionViewModel;
                viewModel.GoToTabView.Execute(tab);
            }
        }

        //Unfortunately, not possible to trigger this event via ViewModel
        private void TabList_IsSelectionEnabledChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {  
            var viewModel = DataContext as CollectionViewModel;
            viewModel.SetIsSelectionEnabled.Execute(sender);       
        }

       

    }
}
