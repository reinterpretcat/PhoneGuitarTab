using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using LazyListBox;
using PhoneGuitarTab.Core.Tab;

namespace PhoneGuitarTab.UI.Notation.Infrastructure
{
    public class LazyTrackDataItem : ILazyDataItem, INotifyPropertyChanged
    {
        //data source
        private Core.Tab.Measure _measure;

        //fast data
        private string _number;
        private string _measureWidth;
        //slow data
        private Core.Tab.Measure _view;

        #region properties

        //Lazy loading enabled
        internal bool IsLazy { get; set; }
        public int Index { get; internal set; }

        public string Number
        {
            get
            {
                if (_number != null)
                {
                    return _number;
                }
                else
                {
                    throw new InvalidOperationException("Unexpected call to Number before it was loaded");
                }
            }
            private set
            {
                _number = value;
            }
        }


        public string MeasureWidth
        {
            get
            {
                if (_measureWidth != null)
                {
                    return _measureWidth;
                }
                else
                {
                    throw new InvalidOperationException("Unexpected call to MeasureWidth before it was loaded");
                }
            }
            private set
            {
                _measureWidth = value;
            }
        }


        public Core.Tab.Measure View
        {
            get
            {
                return _view;
            }
            private set
            {
                _view = value;
            }
        }

        #endregion

        public LazyTrackDataItem(int index, Core.Tab.Measure measure)
        {
            Index = ++index;
            _measure = measure;

        }

        #region State

        void LoadSmallAndFastData()
        {
            _number = Index.ToString();
            _measureWidth = MeasureHelper.GetMeasureWidth(_measure).ToString();
                
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs("Number"));
                handler(this, new PropertyChangedEventArgs("MeasureWidth"));
            }
        }

        void BeginLoadLargeAndSlowData()
        {
            Thread t = new Thread(LoadWorker);
            t.Start();

        }

        void LoadWorker()
        {
            Deployment.Current.Dispatcher.BeginInvoke(LoadLargeAndSlowOnUiThread);
        }

        bool pendingWorkLargeAndSlowOnUiThread = false;

        void LoadLargeAndSlowOnUiThread()
        {
            // Paused, so don't use UI thread
            if (IsPaused)
            {
                pendingWorkLargeAndSlowOnUiThread = true;
                return;
            }

            pendingWorkLargeAndSlowOnUiThread = false;

            // State has changed while we were asleep
            if (currentState != LazyDataLoadState.Loading && currentState != LazyDataLoadState.Reloading)
                return;

            _view = _measure;
            

            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs("View"));

            EndLoadSlowData();
        }

        void EndLoadSlowData()
        {
            // State has changed while we were asleep
            if (currentState != LazyDataLoadState.Loading && currentState != LazyDataLoadState.Reloading)
                return;
            GoToState(LazyDataLoadState.Loaded);
        }


        void UnloadLargeData()
        {
            if (IsLazy != true)
            {

                return;
            }
            _view = null;
            pendingWorkLargeAndSlowOnUiThread = false;
            Unpause();

            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs("View"));
            }
        }

        void UnloadAllData()
        {
            if (IsLazy != true)
            {
                return;
            }
            _number = null;
            _view = null;
            pendingWorkLargeAndSlowOnUiThread = false;
        }

        #endregion

        #region interfaces
        public void GoToState(LazyDataLoadState state)
        {
            //LazyDataItemStateManagerHelper.CheckTransition(currentState, state);

            switch (state)
            {
                case LazyDataLoadState.Unloaded:
                    UnloadAllData();
                    break;

                case LazyDataLoadState.Minimum:
                    LoadSmallAndFastData();
                    break;

                case LazyDataLoadState.Loading:
                    if (IsPaused)
                        Unpause();
                    BeginLoadLargeAndSlowData();
                    break;

                case LazyDataLoadState.Loaded:
                    // nothing;
                    break;

                case LazyDataLoadState.Cached:
                    UnloadLargeData();
                    break;

                case LazyDataLoadState.Reloading:
                    if (IsPaused)
                        Unpause();
                    BeginLoadLargeAndSlowData();
                    break;

                default:
                    throw new InvalidOperationException("Unknown current state " + currentState.ToString());
            }

            EventHandler<LazyDataItemStateChangedEventArgs> handler = CurrentStateChanged;
            if (handler != null)
                handler(this, new LazyDataItemStateChangedEventArgs(currentState, state));

            currentState = state;
        }

        public void Pause()
        {
            if (isPaused == true)
            {

                return;
            }
            isPaused = true;

            EventHandler<LazyDataItemPausedStateChangedEventArgs> handler = PauseStateChanged;
            if (handler != null)
                handler(this, new LazyDataItemPausedStateChangedEventArgs(true));
        }

        public void Unpause()
        {
            if (isPaused == false)
            {
                return;
            }

            isPaused = false;
            if (pendingWorkLargeAndSlowOnUiThread)
            {
                LoadLargeAndSlowOnUiThread();
            }

            if (PauseStateChanged != null)
                PauseStateChanged(this, new LazyDataItemPausedStateChangedEventArgs(true));
        }

        bool isPaused = false;
        public bool IsPaused
        {
            get { return isPaused; }
        }

        LazyDataLoadState currentState;
        public LazyDataLoadState CurrentState
        {
            get { return currentState; }
        }

        public event EventHandler<LazyDataItemStateChangedEventArgs> CurrentStateChanged;

        public event EventHandler<LazyDataItemPausedStateChangedEventArgs> PauseStateChanged;

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}
