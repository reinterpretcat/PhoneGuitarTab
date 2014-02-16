namespace PhoneGuitarTab.Core.Views
{
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using System.Collections.Generic;

    [DataContract]
    public class ViewModel : INotifyPropertyChanged
    {
        private Dictionary<string, object> _navigationParameters = null;
        public Dictionary<string, object> NavigationParameters
        {
            get
            {
                return _navigationParameters;
            }
            set
            {
                _navigationParameters = value;
                ReadNavigationParameters();
            }
        }

        protected virtual void ReadNavigationParameters()
        {
        }

        /// <summary>
        /// Saves view model state into dictionary
        /// </summary>
        public virtual void SaveStateTo(IDictionary<string, object> state)
        {
          
        }

        /// <summary>
        /// Restores view model state from dictionary
        /// </summary>
        public virtual void LoadStateFrom(IDictionary<string, object> state)
        {

        }

        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(name));
        }

        #endregion

    }
}
