using System;
using System.IO;
using System.Windows;
using PhoneGuitarTab.Core;
using PhoneGuitarTab.Data;

namespace PhoneGuitarTab.UI.ViewModel
{
    public class TextTabViewModel : Core.ViewModel
    {

        public string TabContent { get; set; }

        protected override void ReadNavigationParameters()
        {
            try
            {
                IDataContextService database = Container.Resolve<IDataContextService>();
                Tab tab = (Tab) NavigationParameters["Tab"];
                tab.LastOpened = DateTime.Now;
                database.SubmitChanges();
                IFileSystemService service = Container.Resolve<IFileSystemService>();

                using (var stream = service.OpenFile(tab.Path, FileMode.Open))
                    TabContent = String.Format(@"<html><body>{0}</body></html>", (new StreamReader(stream)).ReadToEnd());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
