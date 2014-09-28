using System;
using System.Data.Linq;

namespace PhoneGuitarTab.UI.Data
{
    public interface IDataContextService
    {
        /// <summary>
        ///     Specify a table for the songs.
        /// </summary>
        ITable<Tab> Tabs { get; }

        /// <summary>
        ///     Specify a table for the song types.
        /// </summary>
        ITable<TabType> TabTypes { get; }

        /// <summary>
        ///     Specify a table for the groups.
        /// </summary>
        ITable<Group> Groups { get; }

        void SubmitChanges();

        event EventHandler<EventArgs> OnChanged;

        void InsertTab(Tab tab);

        void DeleteTabById(int id);

        Group GetOrCreateGroupByName(string name);

        TabType GetTabTypeByName(string name);

        Tab GetTabById(int id);
        void UpdateTabMediaById(int tabId, string albumCover);
        void UpdateGroupMediaByName(string name, string normal, string large, string extraLarge);

        bool IsGroupExists(string name);
    }
}