using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using PhoneGuitarTab.UI.Notation.ViewModel;

namespace PhoneGuitarTab.UI.Notation
{
    public partial class TextTabView : PhoneApplicationPage
    {
        public TextTabView()
        {
            InitializeComponent();
            tabWebBrowser.Loaded += delegate
                {
                    tabWebBrowser.NavigateToString((DataContext as TextTabViewModel).TabContent);
                };
        }
    }
}