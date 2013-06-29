using System;

namespace PhoneGuitarTab.UI.Entities
{
    public class TabEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Group { get; set; }
        public string Type { get; set; }
        public string Rating { get; set; }
        public string Path { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public Nullable<DateTime> LastOpened { get; set; }

        public string SearchId { get; set; }
        public string SearchUrl { get; set; }
    }
}
