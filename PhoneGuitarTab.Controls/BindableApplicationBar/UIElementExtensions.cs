using System.Windows;
using System.Windows.Media;

using Microsoft.Phone.Controls;

namespace PhoneGuitarTab.Controls
{
    public static class UIElementExtensions
    {
        public static bool IsInVisualTree(this UIElement element)
        {
            DependencyObject dob = element;
            DependencyObject parent = VisualTreeHelper.GetParent(dob);

            while (parent != null)
            {
                dob = parent;
                parent = VisualTreeHelper.GetParent(dob);
            }

            return dob is PhoneApplicationFrame;
        }
    }
}
