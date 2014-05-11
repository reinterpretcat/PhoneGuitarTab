using System.Collections.Generic;

namespace PhoneGuitarTab.Tablatures.Models.Effects
{
    public class EffectBend
    {
        public static int SemitoneLength = 1;
        public static int MaxPositionLength = 12;
        public static int MaxValueLength = (SemitoneLength*12);

        public List<BendPoint> Points { get; set; }

        public EffectBend()
        {
            Points = new List<BendPoint>();
        }

        public void AddPoint(int position, int value)
        {
            Points.Add(new BendPoint(position, value));
        }

        public class BendPoint
        {
            public int Position { get; set; }
            public int Value { get; set; }

            public BendPoint(int position, int value)
            {
                Position = position;
                Value = value;
            }
        }
    }
}