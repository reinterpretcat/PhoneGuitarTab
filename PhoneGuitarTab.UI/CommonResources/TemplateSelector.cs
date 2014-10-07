using System.Windows;
using System.Windows.Controls;

namespace PhoneGuitarTab.UI.CommonResources
{
    public abstract class TemplateSelector:ContentControl
    {
        public abstract DataTemplate SelectTemplate(object item, DependencyObject container);

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);

            ContentTemplate = SelectTemplate(newContent, this);
        }
    }
}
