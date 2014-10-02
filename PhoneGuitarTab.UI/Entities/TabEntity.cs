using System;
using System.ComponentModel;
using System.Windows;

namespace PhoneGuitarTab.UI.Entities
{
    public class TabEntity : INotifyPropertyChanged
    {
        #region Fields

        //Flag to get theme [to set RelativeimageUri depending on the phone theme]
        private readonly bool dark = ((Visibility) Application.Current.Resources["PhoneDarkThemeVisibility"] ==
                                      Visibility.Visible);

        private bool actionAreaVisible;
        private bool isDownloaded;
        private Uri relativeImageUri;
        private string imageUrl;
        private string albumCoverImageUrl;
        private int id;
        #endregion Fields

        #region Properties

        public int Id 
        { get { return id; }
            set
            {
                id = value;
                RaisePropertyChanged("Id");
            } 
        }
        public string Name { get; set; }
        public string Group { get; set; }
        public string Type { get; set; }
        public string Rating { get; set; }
        public string Path { get; set; }

        public string ImageUrl
        {
            get { return imageUrl; }
            set
            {
                OnImageUrlChanging(value);
                imageUrl = value;
            }
        }

        public Uri RelativeImageUri
        {
            get { return relativeImageUri; }
            set
            {
                relativeImageUri = value;
                RaisePropertyChanged("RelativeImageUri");
            }
        }

        public string AlbumCoverImageUrl
        {
            get { return albumCoverImageUrl; }
            set
            {
                albumCoverImageUrl = value;
                RaisePropertyChanged("AlbumCoverImageUrl");
            }
        }

        public string Description { get; set; }
        public DateTime LastOpened { get; set; }

        public string SearchId { get; set; }
        public string SearchUrl { get; set; }

        public string Version { get; set; }
        public int Votes { get; set; }

        public bool ActionAreaVisible
        {
            get { return actionAreaVisible; }
            set
            {
                actionAreaVisible = value;
                RaisePropertyChanged("ActionAreaVisible");
            }
        }

        public bool IsDownloaded
        {
            get { return isDownloaded; }
            set
            {
                isDownloaded = value;
                RaisePropertyChanged("IsDownloaded");
                RaisePropertyChanged("IsNotDownloaded");
            }
        }

        public bool IsNotDownloaded
        {
            get { return !isDownloaded; }
        }

        #endregion Properties

        #region INotifyPropertyChanged members

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion INotifyPropertyChanged members

        #region ExtensibilityMethods

        private void OnImageUrlChanging(string value)
        {
            if (dark)
                RelativeImageUri = new Uri(value + "_dark.png", UriKind.Relative);
            else
                relativeImageUri = new Uri(value + "_light.png", UriKind.Relative);
        }

        #endregion
    }
}