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

namespace PhoneGuitarTab.UI.Notation.Tests
{
    public class DbTest
    {
        public static void InitDb()
        {
           /* Group all = new Group()
            {
                Id = 0,
                Name = "[All]"
            };*/

            Group gt = new Group()
            {
                Id = RepositoryHelper.GetId<Group>(),
                Name = "Gods Tower"
            };
            Group opeth = new Group()
            {
                Id = RepositoryHelper.GetId<Group>(),
                Name = "Opeth"
            };
            Tab tab = new Tab()
            {
                Id = RepositoryHelper.GetId<Tab>(),
                Group = gt,
                Name = "Evil",
                Rating = "5",
                Type = "guitar pro",
                Path = "MyIsolatedstorage"
            };
            //SterlingService.Current.Database.Save(all);
            SterlingService.Current.Database.Save(gt);
            SterlingService.Current.Database.Save(opeth);
            SterlingService.Current.Database.Save(tab);

           /* AddGroupToDb("Metallica");
            AddGroupToDb("Nest");
            AddGroupToDb("Iron Maiden");
            AddGroupToDb("Helloween");
            AddGroupToDb("Death");
            AddGroupToDb("Burzum");
            AddGroupToDb("Moonsorrow");
            AddGroupToDb("Summoning");
            AddGroupToDb("Deep Purple");
            AddGroupToDb("Accept");*/
        }

        private static void AddGroupToDb(string name)
        {
            var group = new Group()
                            {
                                Id = RepositoryHelper.GetId<Group>(),
                                Name = name
                            };
            SterlingService.Current.Database.Save(group);
            Random rnd = new Random(DateTime.Now.Millisecond);
            UInt16 ggg = (UInt16) new Random(DateTime.Now.Millisecond).Next(10);
            for(int i=0;i<ggg;i++)
            {
                SterlingService.Current.Database.Save(new Tab()
                {
                    Id = RepositoryHelper.GetId<Tab>(),
                    Group = group,
                    Name = RandomString(12-i),
                    //String.Format("{0}-{1}",group.Name,i),
                    Rating = rnd.Next(1,5).ToString(),
                    Type = "guitar pro",
                    Path = "MyIsolatedstorage"
                });
            }
        }

        private static readonly Random _rng = new Random();
        private  const string _chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        private static string RandomString(int size)
        {
            char[] buffer = new char[size];

            for (int i = 0; i < size; i++)
            {
                buffer[i] = _chars[_rng.Next(_chars.Length)];
            }
            return new string(buffer);
        }

    }
}
