using System.Collections.Generic;

namespace PhoneGuitarTab.Tablatures.Models.Effects
{
    public class EffectTremoloBar
    {
        public List<TremoloBarPoint> Points { get; set; }

        public EffectTremoloBar()
        {
            Points = new List<TremoloBarPoint>();
        }

        public void AddPoint(int position, int value)
        {
            Points.Add(new TremoloBarPoint(position, value));
        }

        public class TremoloBarPoint
        {
            public int Position { get; set; }
            public int Value { get; set; }

            public TremoloBarPoint(int position, int value)
            {
                Position = position;
                Value = value;
            }
        }
    }
}