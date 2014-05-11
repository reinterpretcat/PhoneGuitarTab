namespace PhoneGuitarTab.Tablatures.Models
{
    public class Stroke
    {
        public static int StrokeNone = 0;
        public static int StrokeUp = 1;
        public static int StrokeDown = -1;

        public int Direction { get; set; }
        public int Value { get; set; }

        public Stroke()
        {
            Direction = StrokeNone;
        }
    }
}