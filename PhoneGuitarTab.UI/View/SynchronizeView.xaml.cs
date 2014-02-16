using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using PhoneGuitarTab.UI.ViewModel;
namespace PhoneGuitarTab.UI.View
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