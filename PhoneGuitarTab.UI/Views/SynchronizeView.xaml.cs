using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using PhoneGuitarTab.UI.ViewModels;

namespace PhoneGuitarTab.UI.Views
{
    public partial class SynchronizeView : PhoneApplicationPage
    {
        public SynchronizeView()
        {
            InitializeComponent();
            var viewModel = DataContext as SynchronizeViewModel;
            SystemTray.SetProgressIndicator(this, viewModel.ProgressIndicator);
        }
    }
}