using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using PhoneGuitarTab.Core.Dependencies;
using PhoneGuitarTab.Search.Audio;
using PhoneGuitarTab.UI.Data;
using PhoneGuitarTab.UI.Infrastructure;

namespace PhoneGuitarTab.UI.ViewModels
{
    public class TextTabViewModel : TabViewModelBase
    {
        public string Style { get; set; }
        private readonly TextTabAdapter _textTabAdapter;

        [Dependency]
        public TextTabViewModel(IAudioSearcherFactory audioSearcherFactory, IDataContextService database, PopUpMessageService popUpMessageService, ConfigService configService, MessageHub hub)
            : base(audioSearcherFactory, database, popUpMessageService, configService, hub)
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

                    if (Tablature.TabType.Name == "chords" || Tablature.TabType.Name=="tab")
                    {
                        var phoneAccentBrush = (Color)Application.Current.Resources["PhoneAccentColor"];

                        document = document.Replace("[ch]", "<span onClick='Chord_onClick((this.textContent || this.innerText))' style='color:#" + phoneAccentBrush.ToString().Substring(3) + ";  font-weight: bold; '>");
                        document = document.Replace("[/ch]", "</span>");
                    }

                    TabContent = document;
                }
            }
            catch (Exception ex)
            {
                Dialog.Show(ex.Message);
                throw ex;
            }
        }
    }
}