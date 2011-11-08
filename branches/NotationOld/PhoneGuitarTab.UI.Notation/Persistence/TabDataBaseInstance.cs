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
using PhoneGuitarTab.UI.Notation.Persistence;
using Wintellect.Sterling.Database;
using System.Collections.Generic;

namespace PhoneGuitarTab.UI.Notation.Persistence
{
	public class TabDataBaseInstance : BaseDatabaseInstance
	{
        public const string GROUP_NAME = "Group_Name";
        public const string TAB_NAME = "Tab_Name";
		public override string Name
		{
			get
			{
				return "TablatureDb";
			}
		}

		protected override List<ITableDefinition> _RegisterTables()
		{
			return new List<ITableDefinition> 
			{
				CreateTableDefinition<Group, string>(g => g.Id).WithIndex<Group, string, string>(GROUP_NAME, g => g.Name),
                CreateTableDefinition<Tab, string>(t => t.Id).WithIndex<Tab, string, string>(TAB_NAME, t => t.Name)
			};
		}
	}
}
