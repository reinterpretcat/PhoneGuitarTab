using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace PhoneGuitarTab.UI.Notation.Persistence
{
    internal static class RepositoryHelper
    {
        //public static bool IsDirty { get; set; }
        public static string GetId<T>()
        {
            return Guid.NewGuid().ToString();
        }

        #region Groups

        public static Group GetGroupByName(string name)
        {
            Group group = (from g in SterlingService.Current.Database
                        .Query<Group, string, string>(TabDataBaseInstance.GROUP_NAME)
                    orderby g.Index
                    where g.LazyValue.Value.Name == name
                    select g.LazyValue.Value).FirstOrDefault();
            if (group == null)
            {
                group = new Group(name);
                Save(group);
            }
            return group ;
        }

        public static List<Group> GetAllGroups()
        {
            try
            {
                return (from g in SterlingService.Current.Database
                            .Query<Group, string, string>(TabDataBaseInstance.GROUP_NAME)
                        orderby g.Index
                        select g.LazyValue.Value).ToList();
            }
            catch
            {
                return new List<Group>();
            }


        }

        #endregion

        #region Tabs

        public static Tab GetTabById(string id)
        {
            Tab tab = (from g in SterlingService.Current.Database
                        .Query<Tab, string, string>(TabDataBaseInstance.TAB_NAME)
                           orderby g.Index
                           where g.LazyValue.Value.Id == id
                           select g.LazyValue.Value).FirstOrDefault();
            /*if (tab == null)
            {
                tab = new Tab(name);
                Save(tab);
            }*/
            return tab;
        }
        
        public static List<Tab> GetAllTabs()
        {
            try
            {
                return (from t in SterlingService.Current.Database
                            .Query<Tab, string, string>(TabDataBaseInstance.TAB_NAME)
                        orderby t.Index
                        select t.LazyValue.Value).ToList();
            }
            catch
            {
                return new List<Tab>();
            }

        }

        public static int GetTabsCount(string groupId)
        {
            try
            {
                return SterlingService.Current
                    .Database.Query<Tab, string, string>(TabDataBaseInstance.TAB_NAME)
                    .Where(t => t.LazyValue.Value.Group.Id == groupId).Count();
            }
            catch
            {
                return 0;
            }
        }

        public static List<Tab> GetTabs(string groupId)
        {
            try
            {
                return (from t in SterlingService.Current.Database
             .Query<Tab, string, string>(TabDataBaseInstance.TAB_NAME)
                        orderby t.Index
                        where t.LazyValue.Value.Group.Id == groupId
                        select t.LazyValue.Value).ToList();
            }
            catch {
                return new List<Tab>();
            }

        }

        #endregion

        public static T Load<T>(string id)
        {
            return (T) SterlingService.Current.Database.Load(typeof (T), id);
        }

        public static void Save<T>(T t)
        {
           SterlingService.Current.Database.Save(typeof(T), t);
        }

        public static void RemoveTab(string tabId)
        {
            var tab = GetTabById(tabId);
            if (tab != null)
            {
                Remove<Tab>(tabId);
                var tabCount = GetTabsCount(tab.Group.Id);
                if(tabCount==0)
                    Remove<Group>(tab.Group.Id);
            }
        }

        private static void Remove<T>(string id)
        {
            SterlingService.Current.Database.Delete(typeof (T), id);
        }
    }
}
