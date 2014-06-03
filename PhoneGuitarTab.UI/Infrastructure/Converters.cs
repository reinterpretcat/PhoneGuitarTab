using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using PhoneGuitarTab.Search;
using PhoneGuitarTab.Search.Tabs;
using PhoneGuitarTab.UI.Entities;
using PhoneGuitarTab.UI.Resources;

namespace PhoneGuitarTab.UI.Infrastructure
{
    public class GroupToBrushValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            BandInGroup group = value as BandInGroup;
            object result = null;

            if (group != null)
            {
                if (group.Count == 0)
                {
                    result = Application.Current.Resources["PhoneChromeBrush"];
                }
                else
                {
                    result = Application.Current.Resources["PhoneAccentBrush"];
                }
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class IntToStringConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class StringToIntConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value == null ? 0 : Int32.Parse(value.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class TabToBrushValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            TabInGroup group = value as TabInGroup;
            object result = null;

            if (group != null)
            {
                if (group.Count == 0)
                {
                    result = Application.Current.Resources["PhoneChromeBrush"];
                }
                else
                {
                    result = Application.Current.Resources["PhoneAccentBrush"];
                }
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class UpperStringConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.ToString().ToUpper();
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Visibility.Collapsed;

            var isVisible = (bool) value;

            return isVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var visiblity = (Visibility) value;

            return visiblity == Visibility.Visible;
        }
    }

    public class IntToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isVisible = (int) value != 0;
            return isVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("ConvertBack is not supported.");
        }
    }

    public class SearchTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string returnString = string.Empty;
            var searchType = (SearchType) value;
            switch (searchType)
            {
                case SearchType.ByBand:
                    returnString = AppResources.SearchType_Band;
                    break;
                case SearchType.BySong:
                    returnString = AppResources.SearchType_Song;
                    break;
                case SearchType.BandSong:
                    returnString = AppResources.SearchType_BandSong;
                    break;
            }
            return returnString;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("Convert from string to se");
        }
    }

    public class SearchTabTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string returnString = string.Empty;
            var searchType = (TabulatureType) value;
            switch (searchType)
            {
                case TabulatureType.All:
                    returnString = AppResources.SearchTabType_All;
                    break;
                case TabulatureType.Guitar:
                    returnString = AppResources.SearchTabType_Guitar;
                    break;
                case TabulatureType.Bass:
                    returnString = AppResources.SearchTabType_Bass;
                    break;
                case TabulatureType.Chords:
                    returnString = AppResources.SearchTabType_Chords;
                    break;
                case TabulatureType.Drum:
                    returnString = AppResources.SearchTabType_Drum;
                    break;
                case TabulatureType.GuitarPro:
                    returnString = AppResources.SearchTabType_GuitarPro;
                    break;
            }
            return returnString;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("Convert from string to se");
        }
    }

    public class SearchTabTypeImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var searchType = (TabulatureType) value;
            switch (searchType)
            {
                case TabulatureType.All:
                    return new BitmapImage(new Uri("/Images/instrument/musicxml_tile.png", UriKind.Relative));
                case TabulatureType.Guitar:
                    return new BitmapImage(new Uri("/Images/instrument/Electric-Guitar_dark.png", UriKind.Relative));
                case TabulatureType.Bass:
                    return new BitmapImage(new Uri("/Images/instrument/Bass_dark.png", UriKind.Relative));
                case TabulatureType.Chords:
                    return new BitmapImage(new Uri("/Images/instrument/Chords_dark.png", UriKind.Relative));
                case TabulatureType.Drum:
                    return new BitmapImage(new Uri("/Images/instrument/Drums_dark.png", UriKind.Relative));
                case TabulatureType.GuitarPro:
                    return new BitmapImage(new Uri("/Images/instrument/Guitarpro_dark.png", UriKind.Relative));
                default:
                    return new Uri("", UriKind.Relative);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("Convert from string to se");
        }
    }

    public class TabsGroupsCollectionToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var hasTabs = ((IEnumerable<TabInGroup>) value).Any(g => g.HasItems);
            return hasTabs ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("ConvertBack is not supported.");
        }
    }

    public class RelativeDateTimeConverter : IValueConverter
    {
        private const int Minute = 60;
        private const int Hour = Minute*60;
        private const int Day = Hour*24;
        private const int Year = Day*365;

        private readonly Dictionary<long, Func<TimeSpan, string>> thresholds = new Dictionary
            <long, Func<TimeSpan, string>>
        {
            {2, t => AppResources.RelativeDate_Second},
            {Minute, t => String.Format(AppResources.RelativeDate_Seconds, (int) t.TotalSeconds)},
            {Minute*2, t => AppResources.RelativeDate_Minute},
            {Hour, t => String.Format(AppResources.RelativeDate_Minutes, (int) t.TotalMinutes)},
            {Hour*2, t => AppResources.RelativeDate_Hour},
            {Day, t => String.Format(AppResources.RelativeDate_Hours, (int) t.TotalHours)},
            {Day*2, t => AppResources.RelativeDate_Day},
            {Day*30, t => String.Format(AppResources.RelativeDate_Days, (int) t.TotalDays)},
            {Day*60, t => AppResources.RelativeDate_Month},
            {Year, t => String.Format(AppResources.RelativeDate_Months, (int) t.TotalDays/30)},
            {Year*2, t => AppResources.RelativeDate_Year},
            {Int64.MaxValue, t => String.Format(AppResources.RelativeDate_Years, (int) t.TotalDays/365)}
        };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dateTime = (DateTime) value;
            var difference = DateTime.UtcNow - dateTime.ToUniversalTime();

            return thresholds.First(t => difference.TotalSeconds < t.Key).Value(difference);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class ImageUrlToUriConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && !String.IsNullOrEmpty(value.ToString()))
                return new Uri(value.ToString(), UriKind.RelativeOrAbsolute);
            else
                return new Uri("", UriKind.RelativeOrAbsolute);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var imageUrl = (Uri) value;
            return imageUrl.OriginalString;
        }
    }

    public class ObjectNameToImagePathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && !String.IsNullOrEmpty(value.ToString()))
            {
                bool dark = ((Visibility) Application.Current.Resources["PhoneDarkThemeVisibility"] ==
                             Visibility.Visible);

                if (dark)
                    return new Uri("/Images/dark/" + value.ToString() + "_dark.png", UriKind.Relative);
                else
                    return new Uri("/Images/light/" + value.ToString() + "_light.png", UriKind.Relative);
            }
            else
                return new Uri("", UriKind.Relative);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var imageUrl = (Uri) value;
            return imageUrl.OriginalString;
        }
    }
}