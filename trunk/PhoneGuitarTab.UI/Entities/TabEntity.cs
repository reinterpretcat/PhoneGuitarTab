using System;
using System.ComponentModel;

namespace PhoneGuitarTab.UI.Entities
{
    public class TabEntity : INotifyPropertyChanged
    {
        private bool actionAreaVisible = false;

        public int Id { get; set; }
        public string Name { get; set; }
        public string Group { get; set; }
        public string Type { get; set; }
        public string Rating { get; set; }
        public string Path { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public Nullable<DateTime> LastOpened { get; set; }

        public string SearchId { get; set; }
        public string SearchUrl { get; set; }

        public bool ActionAreaVisible
        {
            get
            {
                return actionAreaVisible;
            }
            set
            {
                actionAreaVisible = value;
                RaisePropertyChanged("ActionAreaVisible");
            }
        }


        #region INotifyPropertyChanged members

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion INotifyPropertyChanged members
    }
}
