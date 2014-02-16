namespace PhoneGuitarTab.Tablatures.Models
{
    public class TimeSignature
    {
        public Duration Denominator { get; set; }
        public int Numerator { get; set; }

        public TimeSignature()
        {
            Numerator = 4;
            Denominator = new Duration();
        }
    }
}