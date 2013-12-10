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
        private TextTabAdapter _textTabAdapter;

        

        [Dependency]
        public TextTabViewModel(IDataContextService database, MessageHub hub)
            : base(database, hub)
        {
            _textTabAdapter = new TextTabAdapter();
        }

        protected override void ReadNavigationParameters()
        {
            try
            {
                base.ReadNavigationParameters();

                using (var stream = FileSystem.OpenFile(Tablature.Path, FileMode.Open))
                {
                    string document = _textTabAdapter.Adapt(new StreamReader(stream).ReadToEnd());

                    if (Tablature.TabType.Name == "chords")
                    {
                        document = document.Replace("[ch]", "<span>");
                        document = document.Replace("[/ch]", "</span>");
                    }

                    TabContent = document;
                }
            }
            catch (Exception ex)
            {
                Dialog.Show(ex.Message);
            }
        }
    }
}
