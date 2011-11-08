using System;
using System.Collections.Generic;
using System.Xml.Linq;


namespace PhoneGuitarTab.Search.Lastfm
{
   
    public class SearchImageResultEntry
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public DateTime DateAdded { get; set; }
        public string Format { get; set; }

        public Owner User { get; set; }
        public List<Size> Sizes { get; set; }
        public Vote Rating { get; set; }

        public SearchImageResultEntry()
        {
            Sizes = new List<Size>();
            Rating = new Vote();
            User = new Owner();
        }

        public static SearchImageResultEntry Create(XElement node)
        {
            SearchImageResultEntry entry = new SearchImageResultEntry
                                               {
                                                   Title = node.Element("title").Value,
                                                   Url = node.Element("url").Value,
                                                   DateAdded = DateTime.Parse(node.Element("dateadded").Value),
                                                   Format = node.Element("format").Value
                                               };

            //process owner
            SearchImageResultEntry.Owner owner = new SearchImageResultEntry.Owner();
            owner.Name = node.Element("owner").Element("name").Value; ;
            owner.Url = node.Element("owner").Element("url").Value; ;
            entry.User = owner;

            //process sizes
            foreach (XElement sizeNode in node.Element("sizes").Elements("size"))
            {
                SearchImageResultEntry.Size size = new SearchImageResultEntry.Size();
                size.Name = sizeNode.Attribute("name").Value; ;
                size.Width = Int32.Parse(sizeNode.Attribute("width").Value);
                size.Height = Int32.Parse(sizeNode.Attribute("height").Value);
                size.Url = sizeNode.Value;
                entry.Sizes.Add(size);
            }

            //process votes
            SearchImageResultEntry.Vote vote = new SearchImageResultEntry.Vote();
            vote.ThumbsUp = Int32.Parse(node.Element("votes").Element("thumbsup").Value);
            vote.ThumbsDown = Int32.Parse(node.Element("votes").Element("thumbsdown").Value);
            entry.Rating = vote;

            return entry;

        }

        #region Nested classes
        public class Owner
        {
            public string Name { get; set; }
            public string Url { get; set; }
        }

        public class Size
        {
            public string Name { get; set; }
            public int Height { get; set; }
            public int Width { get; set; }
            public string Url { get; set; }

        }

        public class Vote
        {
            public int ThumbsUp { get; set; }
            public int ThumbsDown { get; set; }
        }
        #endregion

    }
}
