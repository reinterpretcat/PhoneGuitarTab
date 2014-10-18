using System.Windows;
using PhoneGuitarTab.Controls;
using PhoneGuitarTab.Core.Views;
using PhoneGuitarTab.UI.ViewModels;

namespace PhoneGuitarTab.UI.Views
{
    public partial class StartupView : ViewPage
    {
        private StartupViewModel vm;
        public StartupView()
        {
            InitializeComponent();

            vm = DataContext as StartupViewModel;
            vm.PropertyChanged += viewModel_PropertyChanged;
                                   
        }

        private void viewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // "Bindable.ApplicationBar property needs to be set on a PhoneApplicationPage element."
            //Therefore this piece of code can not be moved to ViewModel
            if (e.PropertyName == "IsSelectionEnabled")
            {
               
                if (vm.IsSelectionEnabled)
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
            UpdateAppBarAndDoUIActions();
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


        private void MainPanorama_OnLoaded(object sender, RoutedEventArgs e)
        {
            UpdateAppBarAndDoUIActions();
        }

        private void UpdateAppBarAndDoUIActions()
        {
            //"Bindable.ApplicationBar property needs to be set on a PhoneApplicationPage element."
            //Therefore this piece of code can not be moved to ViewModel

            //Switch appbars depending on the selected pivot.
            switch (MainPanorama.SelectedIndex)
            {  //suggestions
                case 3:
                    if (vm.IsAdEnabled)
                        Bindable.SetApplicationBar(this, (BindableApplicationBar)Resources["DefaultAdEnabled"]);
                    else
                        Bindable.SetApplicationBar(this, (BindableApplicationBar)Resources["Default"]);

                    if (MainPanorama.Background.Opacity > 0.21)
                        OpacityFadeOut.Begin();
                    break;

                //discover
                case 2:
                    if (vm.IsAdEnabled)
                        Bindable.SetApplicationBar(this, (BindableApplicationBar)Resources["DefaultAdEnabled"]);
                    else
                        Bindable.SetApplicationBar(this, (BindableApplicationBar)Resources["Default"]);
                    if (MainPanorama.Background.Opacity > 0.21)
                        OpacityFadeOut.Begin();
                    vm.RequestBandSuggestion.Execute(null);
                    break;
                //groups
                case 1:
                    if (vm.IsAdEnabled)
                        Bindable.SetApplicationBar(this, (BindableApplicationBar)Resources["DefaultAdEnabled"]);
                    else
                        Bindable.SetApplicationBar(this, (BindableApplicationBar)Resources["Default"]);
                    if (MainPanorama.Background.Opacity > 0.21)
                        OpacityFadeOut.Begin();

                    break;
                //Tabs
                case 0:
                    if (vm.IsAdEnabled)
                        Bindable.SetApplicationBar(this, (BindableApplicationBar)Resources["TabsAppBarAdEnabled"]);
                    else
                        Bindable.SetApplicationBar(this, (BindableApplicationBar)Resources["TabsAppBar"]);
                    if (MainPanorama.Background.Opacity < 0.6)
                        OpacityFadeIn.Begin();
                    break;

                default:
                    if (vm.IsAdEnabled)
                        Bindable.SetApplicationBar(this, (BindableApplicationBar)Resources["DefaultAdEnabled"]);
                    else
                        Bindable.SetApplicationBar(this, (BindableApplicationBar)Resources["Default"]);
                    break;
            }
        }
    }
}