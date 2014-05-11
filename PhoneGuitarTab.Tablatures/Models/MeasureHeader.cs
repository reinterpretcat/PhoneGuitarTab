namespace PhoneGuitarTab.Tablatures.Models
{
    public class MeasureHeader
    {
        public static int TripletFeelNone = 1;
        public static int TripletFeelEighth = 2;
        public static int TripletFeelSixteenth = 3;

        public int Number { get; set; }

        public long Start;

        public TimeSignature TimeSignature { get; set; }

        public Tempo Tempo { get; set; }

        public Marker Marker { get; set; }

        public bool IsRepeatOpen { get; set; }

        public int RepeatAlternative { get; set; }

        public int RepeatClose { get; set; }

        public int TripletFeel { get; set; }

        public Song Song { get; set; }

        public MeasureHeader()
        {
            TripletFeel = TripletFeelNone;
        }
    }
}