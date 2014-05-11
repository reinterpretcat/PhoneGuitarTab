namespace PhoneGuitarTab.Tablatures.Models
{
    public class Color
    {
        public static Color Red = NewColor(255, 0, 0);
        public static Color Green = NewColor(0, 255, 0);
        public static Color Blue = NewColor(0, 0, 255);
        public static Color White = NewColor(255, 255, 255);
        public static Color Black = NewColor(0, 0, 0);

        public int R { get; set; }
        public int G { get; set; }
        public int B { get; set; }

        public Color()
        {
            R = 0;
            G = 0;
            B = 0;
        }

        public static Color NewColor(int r, int g, int b)
        {
            Color color = new Color();
            color.R = r;
            color.G = g;
            color.B = b;
            return color;
        }
    }
}