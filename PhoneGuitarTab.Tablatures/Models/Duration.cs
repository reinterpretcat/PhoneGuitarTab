namespace PhoneGuitarTab.Tablatures.Models
{
    public class Duration
    {
        public const long QuarterTime = 960;
        public const int Whole = 1;
        public const int Half = 2;
        public const int Quarter = 4;
        public const int Eighth = 8;
        public const int Sixteenth = 16;
        public const int ThirtySecond = 32;
        public const int SixtyFourth = 64;

        public int Value { get; set; }

        public bool IsDotted { get; set; }

        public bool IsDoubleDotted { get; set; }

        public DivisionType Division;


        public Duration()
        {
            Value = Quarter;
            IsDotted = false;
            IsDoubleDotted = false;
            Division = new DivisionType();
        }
    }
}