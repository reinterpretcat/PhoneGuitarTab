using System;
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
using System.IO.IsolatedStorage;

namespace PhoneGuitarTab.UI.Notation.Infrastructure
{
	public static class IsolatedStorageHelper
	{

        #region settings

	    private static IsolatedStorageSettings _settings;
		private static IsolatedStorageSettings Settings
		{
			get { return _settings ?? (_settings = IsolatedStorageSettings.ApplicationSettings); }
		}

        public static void SaveParameter(string key, object value)
        {
            Settings[key] = value;
            Settings.Save();
        }

        public static bool IsExistParameter(string key)
        {
            return Settings.Contains(key);
        }

        public static T LoadParameter<T>(string key)
        {
            if (!Settings.Contains(key))
                return default(T);
            else
                return (T)Settings[key];
        }

        #endregion

        #region file store

        private static IsolatedStorageFile _store;
	    public static IsolatedStorageFile Store
	    {
            get { return _store ?? (_store = IsolatedStorageFile.GetUserStoreForApplication()); }
        }

	    private const string TabDirectory = "Tabs";

        public static string CreateTabFilePath()
        {
            if (!Store.DirectoryExists(TabDirectory))
                Store.CreateDirectory(TabDirectory);

            return String.Format("{0}\\{1}.tab", TabDirectory, Guid.NewGuid().ToString());
        }

        public static Stream CreateTabFile(string filePath)
        {
            return IsolatedStorageFile.GetUserStoreForApplication().OpenFile(filePath, FileMode.CreateNew);
        }



	    private const string ImageDirectory = "DownloadedImages";
        public static string CreateImageFilePath(string type)
        {
            if (!Store.DirectoryExists(ImageDirectory))
                Store.CreateDirectory(ImageDirectory);

            return String.Format("{0}\\{1}.{2}", TabDirectory, Guid.NewGuid().ToString(),type);
        }

	    #endregion


	}
}
