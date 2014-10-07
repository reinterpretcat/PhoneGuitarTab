using System.Windows;
using PhoneGuitarTab.Core.Primitives;
using PhoneGuitarTab.UI.Data;
using PhoneGuitarTab.UI.Entities;

namespace PhoneGuitarTab.UI.CommonResources
{
   public class ItemTemplateSelector:TemplateSelector
    {

       public DataTemplate SuggestedBandTemplate { get; set; }
       public DataTemplate BrowseByGenreItemTemplate { get; set; }
       public DataTemplate CollectionHeaderTemplate { get; set; }

       public override DataTemplate SelectTemplate(object item, DependencyObject container)
       {
           var g = item as ObservableTuple<ItemType, Group>;   
       
           if (g.Item1 == ItemType.ByArtist)
                return SuggestedBandTemplate;
           else if (g.Item1 == ItemType.ByGenre)
               return BrowseByGenreItemTemplate;
           else
               return CollectionHeaderTemplate;
           
       }


    }
}
