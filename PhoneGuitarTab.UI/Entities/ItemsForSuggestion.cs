using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Xna.Framework.Media;
using PhoneGuitarTab.Core.Primitives;
using PhoneGuitarTab.Search.Suggestions;
using PhoneGuitarTab.UI.Data;

namespace PhoneGuitarTab.UI.Entities
{
    public class ItemsForSuggestion : ObservableCollection<ObservableTuple<ItemType, Group>>
    {

        public void ClearSuggestedArtists()
        {
            this.Remove(t => t.Item1 == ItemType.ByArtist);
        }

      
    }

  
    public enum ItemType
    {
        ByGenre =0,
        ByArtist =1,
        CollectionHeader=2
    }

    //Extension to remove from observable tuple with lambda
    public static class ExtensionMethods
    {
        public static int Remove<T>(
            this ObservableCollection<T> coll, Func<T, bool> condition)
        {
            var itemsToRemove = coll.Where(condition).ToList();

            foreach (var itemToRemove in itemsToRemove)
            {
                coll.Remove(itemToRemove);
            }

            return itemsToRemove.Count;
        }
    }
}