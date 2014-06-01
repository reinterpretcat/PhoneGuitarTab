using PhoneGuitarTab.Controls;
using PhoneGuitarTab.Core.Views;
using PhoneGuitarTab.UI.ViewModels;

namespace PhoneGuitarTab.UI.Views
{
    public partial class StartupView : ViewPage
    {
        public StartupView()
        {
            InitializeComponent();

            var viewModel = DataContext as StartupViewModel;
            viewModel.PropertyChanged += viewModel_PropertyChanged;
        }

        private void viewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // "Bindable.ApplicationBar property needs to be set on a PhoneApplicationPage element."
            //Therefore this piece of code can not be moved to ViewModel
            if (e.PropertyName == "IsSelectionEnabled")
            {
                var viewModel = sender as StartupViewModel;
                if (viewModel.IsSelectionEnabled)
                    Bindable.SetApplicationBar(this, (BindableApplicationBar) Resources["MultiSelectAppBar"]);
                else
                    Bindable.SetApplicationBar(this, (BindableApplicationBar) Resources["TabsAppBar"]);
            }
        }

        private void StartupView_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var view = sender as StartupView;
            if (view.MainPanorama.SelectedItem.GetType() == typeof (PanaromaItems.Tabs))
            {
                var viewModel =
                    ((view.MainPanorama.SelectedItem) as PanaromaItems.Tabs).DataContext as CollectionViewModel;
                if (viewModel.IsSelectionEnabled)
                {
                    e.Cancel = true;
                    viewModel.IsSelectionEnabled = false;
                }
            }
        }

        private void MainPanorama_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            //"Bindable.ApplicationBar property needs to be set on a PhoneApplicationPage element."
            //Therefore this piece of code can not be moved to ViewModel

            //Switch appbars depending on the selected pivot.
            switch (MainPanorama.SelectedIndex)
            {
                    //recent
                case 2:
                    Bindable.SetApplicationBar(this, (BindableApplicationBar) Resources["Recent"]);
                    if (MainPanorama.Background.Opacity > 0.21)
                        OpacityFadeOut.Begin();
                    break;
                    //groups
                case 1:
                    Bindable.SetApplicationBar(this, (BindableApplicationBar) Resources["Default"]);
                    if (MainPanorama.Background.Opacity > 0.21)
                        OpacityFadeOut.Begin();
                    break;
                    //Tabs
                case 0:
                    Bindable.SetApplicationBar(this, (BindableApplicationBar) Resources["TabsAppBar"]);
                    if (MainPanorama.Background.Opacity < 0.5)
                        OpacityFadeIn.Begin();
                    break;

                default:
                    Bindable.SetApplicationBar(this, (BindableApplicationBar) Resources["Default"]);
                    break;
            }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (_isNewPageInstance)
            {
                if (!e.IsNavigationInitiator)
                    State["TabUrl"] = e.Uri.OriginalString;
                // restore page state
                if (State.Count > 0)
                    (DataContext as ViewModel).LoadStateFrom(State);
            }

            // Set _isNewPageInstance to false. If the user navigates back to this page
            // and it has remained in memory, this value will continue to be false.
            _isNewPageInstance = false;
        }
    }
}