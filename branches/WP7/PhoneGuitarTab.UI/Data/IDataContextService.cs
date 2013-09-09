namespace PhoneGuitarTab.Data
{
    using System;
    using System.Data.Linq;

    using PhoneGuitarTab.Data;

    public interface IDataContextService
    {
        /// <summary>
        /// Specify a table for the songs.
        /// </summary>
        Table<Tab> Tabs { get; }

        /// <summary>
        /// Specify a table for the song types.
        /// </summary>
        Table<TabType> TabTypes { get; }

        /// <summary>
        /// Specify a table for the groups.
        /// </summary>
        Table<Group> Groups { get; }

        void SubmitChanges();

        event EventHandler<EventArgs> OnChanged;

        void InsertTab(Tab tab);

        void DeleteTabById(int id);

        Group GetOrCreateGroupByName(string name);

        TabType GetTabTypeByName(string name);
    }
}
