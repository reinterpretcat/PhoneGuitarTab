using System;
using System.IO;
using PhoneGuitarTab.Core.Dependencies;
using PhoneGuitarTab.Core.Views.Commands;
using PhoneGuitarTab.Search;
using PhoneGuitarTab.Search.Audio;
using PhoneGuitarTab.UI.Data;
using PhoneGuitarTab.UI.Infrastructure;

namespace PhoneGuitarTab.UI.ViewModels
{
    public class StaveTabViewModel : TabViewModelBase
    {

        private bool _scaleRequested;
        public bool ScaleRequested
        {
            get { return _scaleRequested; }
            set
            {
                _scaleRequested = value;
                RaisePropertyChanged("ScaleRequested");
            }
        }

        private bool _instrumentsDemanded;
        public bool InstrumentsDemanded
        {
            get { return _instrumentsDemanded; }
            set
            {
                _instrumentsDemanded = value;
                RaisePropertyChanged("InstrumentsDemanded");
            }
        }

        [Dependency]
        public StaveTabViewModel(IAudioSearcherFactory audioSearcherFactory, IDataContextService database, RatingService ratingService, MessageHub hub)
            : base(audioSearcherFactory, database, ratingService, hub)
        {
            CreateCommands();
        }

        public void NavigateToHome()
        {
            NavigationService.NavigateTo(NavigationViewNames.Startup);
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



        #region Commands

        public ExecuteCommand Scale { get; private set; }
        public ExecuteCommand ToggleShowInstruments { get; private set; }
        #endregion Commands

        #region Command handlers

        private void DoScale()
        {
            this.ScaleRequested = true;
        }
        private void DoToggleShowInstruments()
        {
            this.InstrumentsDemanded = true;
        }

        #endregion Command handlers

        #region helpers
       

        private void CreateCommands()
        {
            Scale = new ExecuteCommand(DoScale);
            ToggleShowInstruments = new ExecuteCommand(DoToggleShowInstruments);
        }

        #endregion
    }
}
