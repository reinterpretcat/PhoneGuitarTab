using System.Windows;
using System.Windows.Controls;
using PhoneGuitarTab.UI.Notation.Infrastructure;

namespace PhoneGuitarTab.UI.Notation
{
    public partial class MeasureView : UserControl
    {

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                "Value", typeof(Core.Tab.Measure),
                typeof(MeasureView), null
                );
        public Core.Tab.Measure Value
        {
            get { return (Core.Tab.Measure)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public MeasureView()
        {
            this.Loaded += new RoutedEventHandler(MeasureView_Loaded);
            InitializeComponent();

        }

        void MeasureView_Loaded(object sender, RoutedEventArgs e)
        {
            var bitmap = MeasureHelper.GetMeasureImage(Value);
            if (bitmap != null)
                image.Source = bitmap;
        }

    }
}
