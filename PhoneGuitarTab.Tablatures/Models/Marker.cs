namespace PhoneGuitarTab.Tablatures.Models
{
    public class Marker
    {
        private static readonly Color DefaultColor = Color.Red;
        private const string DefaultTitle = "Untitled";

        public int Measure { get; set; }
        public string Title { get; set; }
        public Color Color { get; set; }

        public Marker()
        {
            Measure = 0;
            Title = DefaultTitle;
            Color = DefaultColor;
        }
    }
}