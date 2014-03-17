namespace PhoneGuitarTab.UI.View
{
    using PhoneGuitarTab.Core.Views;
    using PhoneGuitarTab.Controls;
    using PhoneGuitarTab.UI.ViewModel;
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
            if (view.MainPivot.SelectedItem.GetType() == typeof (Pivots.Tabs))
            {
                var selector = ((view.MainPivot.SelectedItem) as Pivots.Tabs).TabList as Microsoft.Phone.Controls.LongListMultiSelector;
                var viewModel = ((view.MainPivot.SelectedItem) as Pivots.Tabs).DataContext as CollectionViewModel;
                if (selector.IsSelectionEnabled)
                {
                    e.Cancel = true;
                    selector.IsSelectionEnabled = false;
                    viewModel.SetIsSelectionEnabled.Execute(false);
                }
                
            }
        }

        private void Pivot_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            //"Bindable.ApplicationBar property needs to be set on a PhoneApplicationPage element."
            //Therefore this piece of code can not be moved to ViewModel
            
            //Switch appbars depending on the selected pivot.
            switch (MainPivot.SelectedIndex)
            { 
                case 0:
                    Bindable.SetApplicationBar(this, (BindableApplicationBar)Resources["TabsAppBar"]); 
                    break;
                case 1:
                    Bindable.SetApplicationBar(this, (BindableApplicationBar)Resources["Search"]); 
                    break;
                default:
                    Bindable.SetApplicationBar(this, (BindableApplicationBar)Resources["Default"]); 
                    break;
            }
        }

    }
}