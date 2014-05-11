using System.ComponentModel;

namespace PhoneGuitarTab.Core.Primitives
{
    public class ObservableTuple<T1, T2> : INotifyPropertyChanged
    {
        private T1 item1;

        public ObservableTuple(T1 item1, T2 item2)
        {
            Item1 = item1;
            Item2 = item2;
        }


        public T1 Item1
        {
            get { return item1; }
            set
            {
                item1 = value;
                RaisePropertyChanged("Item1");
            }
        }

        public T2 Item2 { get; set; }

        #region INotifyPropertyChanged members

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string p)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(p));
        }

        #endregion INotifyPropertyChanged members
    }
}