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
        }

        private void Pivot_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
          
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