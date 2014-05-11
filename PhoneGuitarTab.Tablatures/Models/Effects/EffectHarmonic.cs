namespace PhoneGuitarTab.Tablatures.Models.Effects
{
    public class EffectHarmonic
    {
        public int Type { get; set; }

        public int Data { get; set; }

        public EffectHarmonic()
        {
            Data = 0;
        }
    }
}