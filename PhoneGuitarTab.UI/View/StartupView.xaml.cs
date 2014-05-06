namespace PhoneGuitarTab.UI.View
{
    using PhoneGuitarTab.Core.Views;
    using PhoneGuitarTab.Controls;
    using PhoneGuitarTab.UI.ViewModel;
    using Microsoft.Phone.Controls;
    public partial class StartupView : ViewPage
    {
        public StartupView()
        {
            InitializeComponent();

            var viewModel = DataContext as StartupViewModel;
            viewModel.PropertyChanged += viewModel_PropertyChanged;
        }

        void viewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

            // "Bindable.ApplicationBar property needs to be set on a PhoneApplicationPage element."
            //Therefore this piece of code can not be moved to ViewModel
            if (e.PropertyName == "IsSelectionEnabled")
            {
                var viewModel = sender as StartupViewModel;
                if (viewModel.IsSelectionEnabled)
                    Bindable.SetApplicationBar(this, (BindableApplicationBar)Resources["MultiSelectAppBar"]); 
                else
                 Bindable.SetApplicationBar(this, (BindableApplicationBar)Resources["TabsAppBar"]);               
            }

        
           
        }
         
        private void StartupView_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var view = sender as StartupView;
            if (view.MainPanorama.SelectedItem.GetType() == typeof (PanaromaItems.Tabs))
            {
                var viewModel = ((view.MainPanorama.SelectedItem) as PanaromaItems.Tabs).DataContext as CollectionViewModel;
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
                     Bindable.SetApplicationBar(this, (BindableApplicationBar)Resources["Recent"]);
                     if (this.MainPanorama.Background.Opacity > 0.21)
                       this.OpacityFadeOut.Begin();
                    break;
                //groups
                case 1:
                   Bindable.SetApplicationBar(this, (BindableApplicationBar)Resources["Default"]);
                   if (this.MainPanorama.Background.Opacity > 0.21)
                        this.OpacityFadeOut.Begin();
                    break;
                //Tabs
                case 0:
                    Bindable.SetApplicationBar(this, (BindableApplicationBar)Resources["TabsAppBar"]);
                    if(this.MainPanorama.Background.Opacity < 0.5)
                    this.OpacityFadeIn.Begin();
                    break;
              
                default:
                    Bindable.SetApplicationBar(this, (BindableApplicationBar)Resources["Default"]);
                    break;
            }
        }

     

    }
}