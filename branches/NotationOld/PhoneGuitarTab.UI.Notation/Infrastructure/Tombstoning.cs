using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Shell;

namespace PhoneGuitarTab.UI.Notation.Infrastructure
{
	public static class Tombstoning
	{
		public static void Save(string key, object value)
		{
			if (PhoneApplicationService.Current.State.ContainsKey(key))
				PhoneApplicationService.Current.State.Remove(key);

			PhoneApplicationService.Current.State.Add(key, value);
		}

		public static T Load<T>(string key)
		{
			object result;

			if (!PhoneApplicationService.Current.State.TryGetValue(key, out result))
				result = default(T);
			else
				PhoneApplicationService.Current.State.Remove(key);

			return (T)result;
		}
	}
}
