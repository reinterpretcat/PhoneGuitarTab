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

        private void TabList_IsSelectionEnabledChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            var selector = sender as LongListMultiSelector;
            var viewModel = DataContext as CollectionViewModel;
            viewModel.SetIsSelectionEnabled.Execute(selector.IsSelectionEnabled);
            this.TabList.UpdateLayout();
      
        }

        private void TabList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var viewModel = DataContext as CollectionViewModel;
            //viewModel.SelectedItemIds.Clear();
            foreach (TabEntity item  in e.AddedItems)
	         {
                 viewModel.SelectedItemIds.Add(item.Id);                    
	         }

            foreach (TabEntity item in e.RemovedItems)
            {
                viewModel.SelectedItemIds.Remove(item.Id);
            }

        }


    }
}
