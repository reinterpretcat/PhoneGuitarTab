namespace PhoneGuitarTab.Tablatures.Models.Effects
{
    public class EffectTrill
    {
        public int Fret { get; set; }
        public Duration Duration { get; set; }

        public EffectTrill()
        {
            Fret = 0;
            Duration = new Duration();
        }
    }
}