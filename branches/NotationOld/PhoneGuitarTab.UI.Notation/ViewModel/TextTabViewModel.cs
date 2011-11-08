using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using PhoneGuitarTab.Core;
using PhoneGuitarTab.Core.Tab;
using PhoneGuitarTab.UI.Notation.Infrastructure;
using PhoneGuitarTab.UI.Notation.Persistence;

namespace PhoneGuitarTab.UI.Notation.ViewModel
{
    public class TextTabViewModel : ViewModelBase
    {

        public string TabContent { get; set; }

        protected override void ReadNavigationParameters()
        {
            //for history
            var tab = RepositoryHelper.Load<Tab>(NavigationParameters["TabId"].ToString());
            tab.LastOpened = DateTime.Now;
            RepositoryHelper.Save<Tab>(tab);

            try
            {
                using (var stream = IsolatedStorageHelper.Store.OpenFile(tab.Path, FileMode.Open))
                TabContent = String.Format(@"<html><body>{0}</body></html>",(new StreamReader(stream)).ReadToEnd());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
           
        }
    }
}
