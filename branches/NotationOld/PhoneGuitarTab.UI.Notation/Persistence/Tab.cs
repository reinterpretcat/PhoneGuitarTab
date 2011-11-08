using System;
using System.Collections.Generic;
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

namespace PhoneGuitarTab.UI.Notation.Persistence
{
    public class Tab
    {
        public string Id { get; set; }
        public string SearchId { get; set; }
        public string SearchUrl { get; set; }
        public string Name { get; set; }
        public Group Group { get; set; }

        internal static Dictionary<string, string> ImageTypeMapping { get; set; }
        static Tab()
        {
            ImageTypeMapping = new Dictionary<string, string>
                                   {
                                       {"tab pro", "/Images/all/TabGP.png"},
                                       {"power tab", "/Images/all/TabPTB.jpg"},
                                       {"default", "/Images/all/TabText.png"}
                                   };
        }                                                     

        private string _type;
        public string Type
        {
            get { return _type; } 
            set
            {
                _type = value;
                ImageUrl = ImageTypeMapping.ContainsKey(_type) ? 
                    ImageTypeMapping[_type] : ImageTypeMapping["default"];
            }
        }

        public string Rating { get; set; }
        public string Path { get; set; }
        //Used for history
        public DateTime LastOpened { get; set; }

       /* public string RatingUrl
        {
            get { return String.Format("/Images/stars/{0}stars.png", Rating); }
        }*/

        public string ImageUrl { get; private set; }
        public Tab()
        {
            Id = RepositoryHelper.GetId<Tab>();
            ImageUrl = "/Images/all/Tab.png";
            LastOpened = DateTime.MinValue;
        }

        public static int CompareByName(object obj1, object obj2)
        {
            Tab p1 = (Tab)obj1;
            Tab p2 = (Tab)obj2;

            return p1.Name.CompareTo(p2.Name);
        }

        public static string GetNameKey(Tab tab)
        {
            char key = char.ToLower(tab.Name[0]);

            if (key < 'a' || key > 'z')
            {
                key = '#';
            }

            return key.ToString();
        }
    }
}
