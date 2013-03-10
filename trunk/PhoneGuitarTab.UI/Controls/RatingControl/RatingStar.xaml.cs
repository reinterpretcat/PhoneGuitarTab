using System.Windows;
using System.Windows.Controls;

namespace PhoneGuitarTab.UI.Controls
{
    public partial class RatingStar : UserControl
    {
        public enum StarState
        {
            Filled,
            Empty
        }

        // Using a DependencyProperty as the backing store for StarState.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register("State", typeof(StarState), typeof(RatingStar), new PropertyMetadata(StarState.Filled));

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(int), typeof(RatingStar), new PropertyMetadata(0));


        public StarState State
        {
            get { return (StarState)GetValue(StateProperty); }
            set
            {
                SetValue(StateProperty, value);

                switch (this.State)
                {
                    case StarState.Empty:
                        this.PathEmpty.Visibility = System.Windows.Visibility.Visible;
                        this.PathFilled.Visibility = System.Windows.Visibility.Collapsed;
                        break;
                    case StarState.Filled:
                        this.PathEmpty.Visibility = System.Windows.Visibility.Collapsed;
                        this.PathFilled.Visibility = System.Windows.Visibility.Visible;
                        break;
                }
            }
        }

        public int Value
        {
            get { return (int)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public RatingStar()
        {
            InitializeComponent();
        }
    }
}
