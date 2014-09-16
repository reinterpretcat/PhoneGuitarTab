using System;
using System.IO;
using System.Windows;
using System.Windows.Automation.Provider;
using System.Windows.Media;
using PhoneGuitarTab.Core.Dependencies;
using PhoneGuitarTab.Search.Audio;
using PhoneGuitarTab.UI.Data;
using PhoneGuitarTab.Search;
using PhoneGuitarTab.UI.Infrastructure;

namespace PhoneGuitarTab.UI.ViewModels
{
    public class TextTabViewModel : TabViewModelBase
    {
        public string Style { get; set; }
        private readonly TextTabAdapter _textTabAdapter;

        [Dependency]
        public TextTabViewModel(IAudioSearcher audioSearcher, IDataContextService database, RatingService ratingService, MessageHub hub)
            : base(audioSearcher, database, ratingService, hub)
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