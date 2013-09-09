using GalaSoft.MvvmLight;
using System.Collections.Generic;

namespace PhoneGuitarTab.Core
{
    public class ViewModel : ViewModelBase
    {
        private Dictionary<string, object> navigationParameters = null;
        public Dictionary<string, object> NavigationParameters
        {
            get
            {
                return navigationParameters;
            }
            set
            {
                navigationParameters = value;
                ReadNavigationParameters();
            }
        }

        protected virtual void ReadNavigationParameters()
        {
        }

        /// <summary>
        /// Saves view model state into dictionary
        /// </summary>
        /// <param name="state"></param>
        public virtual void SaveStateTo(IDictionary<string, object> state)
        {

        }

        /// <summary>
        /// Restores view model state from dictionary
        /// </summary>
        /// <param name="state"></param>
        public virtual void LoadStateFrom(IDictionary<string, object> state)
        {

        }
    }
}
