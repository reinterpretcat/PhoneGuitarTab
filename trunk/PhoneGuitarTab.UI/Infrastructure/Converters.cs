using PhoneGuitarTab.UI.Entities;
using PhoneGuitarTab.UI.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Linq;
using PhoneGuitarTab.Search;

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
                    result = (SolidColorBrush)Application.Current.Resources["PhoneChromeBrush"];
                }
                else
                {
                    result = (SolidColorBrush)Application.Current.Resources["PhoneAccentBrush"];
                }
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
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

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
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
            return Int32.Parse(value.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
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
                    result = (SolidColorBrush)Application.Current.Resources["PhoneChromeBrush"];
                }
                else
                {
                    result = (SolidColorBrush)Application.Current.Resources["PhoneAccentBrush"];
                }
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
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

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
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

            var isVisible = (bool)value;

            return isVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var visiblity = (Visibility)value;

            return visiblity == Visibility.Visible;
        }
    }

    public class IntToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isVisible = (int)value != 0;
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
            SearchType searchType = (SearchType)value;

            switch (searchType)
            {
                case SearchType.ByBand :
                    returnString = "band name";
                    break;
                case SearchType.BySong :
                    returnString = "song name";
                    break;
                default:

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
            TabulatureType searchType = (TabulatureType)value;

            switch (searchType)
            {
                case TabulatureType.All:
                    returnString = "all tabs";
                    break;
                case TabulatureType.Guitar:
                    returnString = "guitar tabs";
                    break;
                case TabulatureType.Bass:
                    returnString = "bass tabs";
                    break;

                default:
                    break;
            }

            return returnString;
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
            var hasTabs = ((IEnumerable<TabInGroup>)value).Any(g => g.HasItems);
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
        private const int Hour = Minute * 60;
        private const int Day = Hour * 24;
        private const int Year = Day * 365;

        private readonly Dictionary<long, Func<TimeSpan, string>> thresholds = new Dictionary<long, Func<TimeSpan, string>>
    {
        {2, t => "a second ago"},
        {Minute,  t => String.Format("{0} seconds ago", (int)t.TotalSeconds)},
        {Minute * 2,  t => "a minute ago"},
        {Hour,  t => String.Format("{0} minutes ago", (int)t.TotalMinutes)},
        {Hour * 2,  t => "an hour ago"},
        {Day,  t => String.Format("{0} hours ago", (int)t.TotalHours)},
        {Day * 2,  t => "yesterday"},
        {Day * 30,  t => String.Format("{0} days ago", (int)t.TotalDays)},
        {Day * 60,  t => "last month"},
        {Year,  t => String.Format("{0} months ago", (int)t.TotalDays / 30)},
        {Year * 2,  t => "last year"},
        {Int64.MaxValue,  t => String.Format("{0} years ago", (int)t.TotalDays / 365)}
    };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dateTime = (DateTime)value;
            var difference = DateTime.UtcNow - dateTime.ToUniversalTime();

            return thresholds.First(t => difference.TotalSeconds < t.Key).Value(difference);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
