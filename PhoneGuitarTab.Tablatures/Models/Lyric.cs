namespace PhoneGuitarTab.Tablatures.Models
{
    public class Lyric
    {
        public int From { get; set; }
        public string Lyrics { get; set; }

        public bool IsEmpty
        {
            get { return string.IsNullOrEmpty(Lyrics); }
        }

        public Lyric()
        {
            From = 1;
            Lyrics = "";
        }
    }
}