using GalaSoft.MvvmLight.Command;
using Microsoft.Phone.Shell;
using PhoneGuitarTab.Core;
using PhoneGuitarTab.Data;
using System;
using System.IO;
using System.Windows;

namespace PhoneGuitarTab.UI.ViewModel
{
    public class TextTabViewModel : Core.ViewModel
    {
        public TextTabViewModel()
        {
            
        }

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
                {
                    string document = (new StreamReader(stream)).ReadToEnd();

                    if (tab.TabType.Name == "chords")
                    {
                        document = document.Replace("[ch]", "<span>");
                        document = document.Replace("[/ch]", "</span>");
                    }

                    TabContent = String.Format("<!DOCTYPE html><html><head><meta name=\"viewport\" content=\"width=device-width; initial-scale=1.0\" /><style>{0}</style></head><body>{1}</body></html>",
                        "span{font-weight:bold;}", 
                        document);
                //unfortunately "initial-scale" attribute is not supported in windows phone 7
                //http://msdn.microsoft.com/en-us/library/ff462082%28VS.92%29.aspx
                //so user need to double tap browser to adjust viewport
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public override void LoadStateFrom(System.Collections.Generic.IDictionary<string, object> state)
        {
            PhoneApplicationService phoneAppService = PhoneApplicationService.Current;
            phoneAppService.UserIdleDetectionMode = IdleDetectionMode.Disabled;

            base.LoadStateFrom(state);
        }

        public override void SaveStateTo(System.Collections.Generic.IDictionary<string, object> state)
        {
            PhoneApplicationService phoneAppService = PhoneApplicationService.Current;
            phoneAppService.UserIdleDetectionMode = IdleDetectionMode.Enabled;

            base.SaveStateTo(state);
        }
    }
}
