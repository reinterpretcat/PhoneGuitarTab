using PhoneGuitarTab.Core;
using PhoneGuitarTab.Data;
using System;
using System.IO;
using System.Windows;

namespace PhoneGuitarTab.UI.ViewModel
{
    public class TextTabViewModel : Core.ViewModel
    {
        public string TabContent { get; set; }

        public string Style { get; set; }

        protected override void ReadNavigationParameters()
        {
            try
            {
                Tab tab = (Tab)NavigationParameters["Tab"];

                Deployment.Current.Dispatcher.BeginInvoke(
                () =>
                {
                    IDataContextService database = Container.Resolve<IDataContextService>();
                    tab.LastOpened = DateTime.Now;
                    database.SubmitChanges();
                });

                IFileSystemService service = Container.Resolve<IFileSystemService>();

                using (var stream = service.OpenFile(tab.Path, FileMode.Open))
                    TabContent = String.Format("<html><head><meta name=\"viewport\" content=\"width=device-width; initial-scale=1.0\" /></head><body style=\"{0}\">{1}</body></html>",
                        Style, (new StreamReader(stream)).ReadToEnd());
                //unfortunately "initial-scale" attribute is not supported in windows phone 7
                //http://msdn.microsoft.com/en-us/library/ff462082%28VS.92%29.aspx
                //so user need to double tap browser to adjust viewport
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
