using System;
using System.IO;
using PhoneGuitarTab.Core.Dependencies;
using PhoneGuitarTab.UI.Data;
using PhoneGuitarTab.UI.Infrastructure;

namespace PhoneGuitarTab.UI.ViewModels
{
    public class StaveTabViewModel : TabViewModelBase
    {
        [Dependency]
        public StaveTabViewModel(IDataContextService database, RatingService ratingService, MessageHub hub)
            : base(database, ratingService, hub)
        {
        }

        public void NavigateToHome()
        {
            NavigationService.NavigateTo(Strings.Startup);
        }

        protected override void ReadNavigationParameters()
        {
            try
            {
                base.ReadNavigationParameters();

                // expect binary format (e.g. gp5)
                using (var stream = FileSystem.OpenFile(Tablature.Path, FileMode.Open))
                {
                    var bytes = default(byte[]);
                    using (var memstream = new MemoryStream())
                    {
                        var buffer = new byte[512];
                        var bytesRead = default(int);
                        while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                            memstream.Write(buffer, 0, bytesRead);
                        bytes = memstream.ToArray();
                    }

                    TabContent = Convert.ToBase64String(bytes);
                }
            }
            catch (Exception ex)
            {
                Dialog.Show(ex.Message);
            }
        }
       
        //Container Class for Tracks
        public class Track
        {
            public string InstrumentName { get; set; }
            public int TrackIndex { get; set; }
            public string Scale { get; set; }

            public Track() { }

            public Track(string instrumenName, int trackIndex, string scale)
            {
                InstrumentName = instrumenName;
                TrackIndex = trackIndex;
                Scale = scale;
            }
        }
    }
}
