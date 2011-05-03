using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using PhoneGuitarTab.Core;
using PhoneGuitarTab.Core.Tab;
using PhoneGuitarTab.UI.Notation.Infrastructure;
using PhoneGuitarTab.UI.Notation.ViewModel;
using UIExtensionMethods;

namespace PhoneGuitarTab.UI.Notation
{
    public partial class MainPage : PhoneApplicationPage
    {
        private TabFile file;
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            this.Loaded += (o, e) =>
                              {
                                  var vm = (DataContext as TabViewModel);
                                  if (vm != null)
                                  {
                                      if (vm.Tablature == null)
                                      {
                                          new NavigationServiceEx().NavigateTo(PageType.Startup);
                                          return;
                                      }
                                      file = vm.Tablature;
                                      //moved to ViewModel
                                      //trackPanel.ItemsSource = file.Tracks;
                                      //myList.ItemsSource = new VirtualizedTrackList(file.Tracks[0], true);
                                      RenderStrings(file.Tracks[0]);
                                  }
                              };
            
        }

       

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            State["scrollOffset"] = myList.GetVerticalScrollOffset();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            object dummy;
            if (State.TryGetValue("scrollOffset", out dummy))
                myList.SetVerticalScrollOffset((double)dummy);
        }

        private void TrackButtonClick(object sender, RoutedEventArgs e)
        {
            var button = e.OriginalSource as Button;
            var index = (int) button.Content;
            //string name = (string) button.Content;
            //var index = file.Tracks.Where(t => t.Name.Contains(name)).SingleOrDefault().Index;
            myList.ItemsSource = new VirtualizedTrackList(file.Tracks[index], true);
            RenderStrings(file.Tracks[index]);

        }
        //TODO: replace into ViewModel using ItemControl in View
        private void RenderStrings(Core.Tab.Track track)
        {
           StringsGrid.Children.Clear();
           for(int i =0; i<track.StringNumber;i++)
           {
               StringsGrid.Children.Add(
                   new Line()
                       {
                           Opacity = 0.4,
                           X1=0,
                           X2=800,
                           Y1 = 30 + i * Constants.NoteHeight,
                           Y2 = 30 + i * Constants.NoteHeight,
                           Stroke =Constants.StringsBrush,
                           Fill = Constants.StringsBrush,
                           StrokeThickness = 1+i/2
                       });
           }

        }

    }
}