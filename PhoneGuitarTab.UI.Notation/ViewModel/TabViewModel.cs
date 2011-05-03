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
    public class TabViewModel : ViewModelBase
    {

        /// <summary>
        /// Certain track
        /// </summary>
        private TabFile _file;
        public TabFile Tablature
        {
            get
            {
                return _file;
            }
            set
            {
                if (_file == value)
                    return;

                _file = value;
                RaisePropertyChanged(() => Tablature);
            }
        }

        /// <summary>
        /// Current track on track panel
        /// </summary>
        private VirtualizedTrackList _currentTrack;
        public VirtualizedTrackList CurrentTrack
        {
            get { return _currentTrack; }
            set
            {
                if (_currentTrack == value)
                    return;

                _currentTrack = value;
                RaisePropertyChanged(() => CurrentTrack);
            }
        }

        public IList<Track> Tracks
        {
            get
            {
                if (_file == null)
                    return null;
                return _file.Tracks;
            }
        }

        protected override void ReadNavigationParameters()
        {
            //for history
            var tab = RepositoryHelper.Load<Tab>(NavigationParameters["TabId"].ToString());
            tab.LastOpened = DateTime.Now;
            RepositoryHelper.Save<Tab>(tab);

            try
            {
                var stream = IsolatedStorageHelper.Store.OpenFile(tab.Path, FileMode.Open);

                Tablature = TabFactory.CreateFromGp(stream);

                //Tests.TestHelper.GetGp4MediumSample2());
                CurrentTrack = new VirtualizedTrackList(_file.Tracks[0], true);
            }
            catch (Exception ex)
            {

                MessageBox.Show(String.Format("Error {0}. TabSearchId = {1}",ex.Message, tab.SearchId));
            }
           
        }
    }
}
