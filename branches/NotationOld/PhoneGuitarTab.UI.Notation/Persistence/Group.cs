namespace PhoneGuitarTab.UI.Notation.Persistence
{
    public class Group
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }

        public string BackgroundUrl { get; set; }

        public int TabCount { get; set; }

        public Group(string name):this()
        {
            Id = RepositoryHelper.GetId<Group>();
            Name = name;
        }

        public Group()
        {
            ImageUrl = "/Images/all/Group.png";
        }

        public static int CompareByName(object obj1, object obj2)
        {
            Group p1 = (Group)obj1;
            Group p2 = (Group)obj2;

            return p1.Name.CompareTo(p2.Name);
        }

        public static string GetNameKey(Group group)
        {
            char key = char.ToLower(group.Name[0]);

            if (key < 'a' || key > 'z')
            {
                key = '#';
            }

            return key.ToString();
        }
    }
}
