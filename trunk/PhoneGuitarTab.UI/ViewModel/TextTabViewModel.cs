using Microsoft.Phone.Shell;
using PhoneGuitarTab.Data;
using System;
using System.IO;
using System.Windows;

namespace PhoneGuitarTab.UI.ViewModel
{
    using PhoneGuitarTab.Core.Dependencies;
    using PhoneGuitarTab.UI.Infrastructure;

    public class TextTabViewModel : TabViewModelBase
    {
        public string Style { get; set; }

        [Dependency]
        public TextTabViewModel(IDataContextService database, MessageHub hub)
            : base(database, hub)
        {
            
        }

        protected override void ReadNavigationParameters()
        {
            try
            {
                base.ReadNavigationParameters();

                using (var stream = FileSystem.OpenFile(Tablature.Path, FileMode.Open))
                {
                    string document = (new StreamReader(stream)).ReadToEnd();

                    if (Tablature.TabType.Name == "chords")
                    {
                        document = document.Replace("[ch]", "<span>");
                        document = document.Replace("[/ch]", "</span>");
                    }

                    string ScrollerScript = " <script> var delay; function slide(interval) {clearInterval(delay); delay =  setInterval(function() {window.scrollBy(0,1)}, interval); }" +
                                    " function stopSlide(){clearInterval(delay);}  </script>";
                   
                   
                    TabContent = String.Format("<!DOCTYPE html><html><head><meta name=\"viewport\" content=\"width=device-width; initial-scale=1.0\" /><style>{0}</style></head><body>{1}{2}</body></html>",
                        "span{font-weight:bold;}",
                        document, ScrollerScript);
                //unfortunately "initial-scale" attribute is not supported in windows phone 7
                //http://msdn.microsoft.com/en-us/library/ff462082%28VS.92%29.aspx
                //so user need to double tap browser to adjust viewport
                }
            }
            catch (Exception ex)
            {
                Dialog.Show(ex.Message);
            }
        }
    }
}
