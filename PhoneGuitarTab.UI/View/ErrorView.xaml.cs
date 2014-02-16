using System;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;

namespace PhoneGuitarTab.UI.View
{
    public partial class ErrorView : PhoneApplicationPage
    {
        private const int MaxErrorLength = 512;

        public ErrorView()
        {
            InitializeComponent();

            var errorText = App.FatalException.ToString();
            var length = errorText.Length > MaxErrorLength ? MaxErrorLength : errorText.Length;
            ErrorTextBlock.Text = App.FatalException.ToString().Substring(0, length - 1);
        }
    }
}