using Microsoft.Phone.Controls;
using PhoneGuitarTab.UI.ViewModels;

namespace PhoneGuitarTab.UI.Views.PanaromaItems
{
    public partial class Tabs : PanoramaItem
    {
        public Tabs()
        {
            InitializeComponent();
        }

        private void TabList_IsSelectionEnabledChanged(object sender,
            System.Windows.DependencyPropertyChangedEventArgs e)
        {
            var viewModel = DataContext as CollectionViewModel;
            viewModel.SetIsSelectionEnabled.Execute(sender);
        }
    }
}